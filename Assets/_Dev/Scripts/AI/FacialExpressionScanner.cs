using UnityEngine;
using Unity.Sentis;
using System.Linq;
using TMPro;
using System.IO;
using System.Collections;


    public class FacialExpressionScanner : MonoBehaviour
    {
        [Header("Model Settings")]
        [Tooltip("Assign your .sentis or .onnx facial expression model here.")]
        public ModelAsset modelAsset;
        
        [Tooltip("Labels corresponding to the model's output classes.")]
        public string[] emotionLabels = new string[] { "Neutral", "Angry", "Happy", "Bored", "Sad", "Shocked", "Crazy" };

        [Header("Input Settings")]
        [Tooltip("The camera that will capture the character's face.")]
        public Camera faceCamera;
        
        [Tooltip("Width expected by the model (Teachable Machine is usually 224).")]
        public int inputWidth = 224; 
        
        [Tooltip("Height expected by the model.")]
        public int inputHeight = 224;

        [Header("Runtime Info")]
        public string currentEmotion;
        public float confidence;

        private Model runtimeModel;
        private Worker worker;
        private RenderTexture targetTexture;
        [SerializeField] TextMeshProUGUI expressionText;
        [SerializeField] UnityEngine.UI.RawImage displayImage;

        [Header("Performance & Smoothing")]
        [Range(0f, 1f)]
        [Tooltip("Higher = faster reaction, Lower = smoother/less flicker.")]
        public float smoothing = 0.15f;
        private float[] smoothedProbabilities;
        
        // Throttling to prevent performance drop
        private float timer;
        public float scanInterval = 0.1f; // 10 scans per second

        [Header("Training Capture")]
        public KeyCode captureKey = KeyCode.X;
        public string subfolderName = "Neutral";
        private int captureCount = 0;

        void Start()
        {
            if (modelAsset == null)
            {
                Debug.LogError("[FacialExpressionScanner] Please assign an ONNX model asset.", this);
                enabled = false;
                return;
            }

            if (faceCamera == null)
            {
                Debug.LogError("[FacialExpressionScanner] Please assign a Camera that is looking at the character's face.", this);
                enabled = false;
                return;
            }

            // 1. Load the model
            runtimeModel = ModelLoader.Load(modelAsset);
            
            // 2. Create the inference engine (Worker)
            // BackendType.GPUCompute is usually best for runtime inference
            worker = new Worker(runtimeModel, BackendType.GPUCompute);

            // 3. Setup Render Texture to capture the camera view
            targetTexture = new RenderTexture(inputWidth, inputHeight, 24);
            faceCamera.targetTexture = targetTexture;

            // Update display image if assigned
            if (displayImage != null)
            {
                displayImage.texture = targetTexture;
            }
            
            Debug.Log("[FacialExpressionScanner] Initialized and ready.");
        }

        void Update()
        {
            // Capture training images
            if (Input.GetKeyDown(captureKey))
            {
                StartCoroutine(CaptureSequence());
            }

            timer += Time.deltaTime;
            if (timer >= scanInterval)
            {
                timer = 0;
                ScanFace();
            }
        }

        IEnumerator CaptureSequence()
        {
            // Take 2 pictures
            CaptureAndSave();
            yield return new WaitForSeconds(0.1f);
            CaptureAndSave();
            Debug.Log($"[FacialExpressionScanner] Captured 2 images for: {subfolderName}");
        }

        void CaptureAndSave()
        {
            if (targetTexture == null) return;

            // 1. Create directory if missing
            string path = Path.Combine(Application.dataPath, "AI_Training_Data", subfolderName);
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            // 2. Read pixels from RenderTexture
            RenderTexture.active = targetTexture;
            Texture2D tex = new Texture2D(targetTexture.width, targetTexture.height, TextureFormat.RGB24, false);
            tex.ReadPixels(new Rect(0, 0, targetTexture.width, targetTexture.height), 0, 0);
            tex.Apply();
            RenderTexture.active = null;

            // 3. Save as PNG
            byte[] bytes = tex.EncodeToPNG();
            string fileName = $"img_{System.DateTime.Now:HHmmss}_{captureCount++}.png";
            File.WriteAllBytes(Path.Combine(path, fileName), bytes);

            Destroy(tex);
        }

        void ScanFace()
        {
            if (worker == null || targetTexture == null) return;

            // 1. Convert the Render Texture to a Tensor in NHWC format (Batch, Height, Width, Channels)
            // Teachable Machine models (Keras) expect NHWC layout and 3 channels (RGB).
            var transform = new TextureTransform()
                .SetDimensions(inputWidth, inputHeight, 3)
                .SetTensorLayout(TensorLayout.NHWC);
            
            using Tensor<float> inputTensor = TextureConverter.ToTensor(targetTexture, transform);

            // 2. Execute the model
            worker.Schedule(inputTensor);

            // 3. Get the output
            Tensor<float> outputTensor = worker.PeekOutput() as Tensor<float>;
            
            if (outputTensor == null) return;

            // 4. Download result to CPU to read it
            // DownloadToArray() handles the readback from GPU
            float[] logits = outputTensor.DownloadToArray();
            float[] probabilities = Softmax(logits);

            // 5. Temporal Smoothing
            if (smoothedProbabilities == null || smoothedProbabilities.Length != probabilities.Length)
            {
                smoothedProbabilities = (float[])probabilities.Clone();
            }
            else
            {
                for (int i = 0; i < probabilities.Length; i++)
                {
                    smoothedProbabilities[i] = Mathf.Lerp(smoothedProbabilities[i], probabilities[i], smoothing);
                }
            }

            // 6. Find the highest probability
            int maxIndex = -1;
            float maxVal = -1.0f;

            for (int i = 0; i < smoothedProbabilities.Length; i++)
            {
                if (smoothedProbabilities[i] > maxVal)
                {
                    maxVal = smoothedProbabilities[i];
                    maxIndex = i;
                }
            }

            // 7. Update public fields
            if (maxIndex >= 0 && maxIndex < emotionLabels.Length)
            {
                currentEmotion = emotionLabels[maxIndex];
                confidence = maxVal;
                expressionText.text = currentEmotion;
            }
        }

        void OnDestroy()
        {
            worker?.Dispose();
            if (targetTexture != null) targetTexture.Release();
        }

        private float[] Softmax(float[] values)
        {
            float max = values.Max();
            float[] result = values.Select(v => Mathf.Exp(v - max)).ToArray();
            float sum = result.Sum();
            for (int i = 0; i < result.Length; i++)
            {
                result[i] /= sum;
            }
            return result;
        }
    }
