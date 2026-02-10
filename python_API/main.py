from fastapi import FastAPI, File, UploadFile
from fastapi.middleware.cors import CORSMiddleware
import torch
import torch.nn as nn
from torchvision import transforms, models
from PIL import Image
import io
import json

app = FastAPI()

app.add_middleware(
    CORSMiddleware,
    allow_origins=[
        "http://localhost:4200",
        "http://127.0.0.1:4200",
    ],
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)

transform = transforms.Compose([
    transforms.Resize((224, 224)),
    transforms.ToTensor(),
    transforms.Normalize([0.485, 0.456, 0.406], [0.229, 0.224, 0.225])
])

with open('./python_API/class_names.json', 'r') as f:
    class_names = json.load(f)

model = models.resnet18(weights=None)
num_ftrs = model.fc.in_features
model.fc = nn.Linear(num_ftrs, len(class_names))

model.load_state_dict(torch.load("./python_API/plant_model_pytorch.pth", map_location=torch.device('cpu')))
model.eval()

@app.post("/predict")
async def predict(file: UploadFile = File(...)):
    image_data = await file.read()
    image = Image.open(io.BytesIO(image_data)).convert('RGB')
    
    image = transform(image).unsqueeze(0) 
    
    with torch.no_grad():
        outputs = model(image)
        probabilities = torch.nn.functional.softmax(outputs, dim=1)[0] * 100
        confidences, class_ids = torch.topk(probabilities, 2)

        predictions_list = []
        for i in range(len(class_ids)):
            predictions_list.append({
                "disease": class_names[class_ids[i].item()],
                "confidence": f"{confidences[i].item():.2f}%"
            })

    return {
        "results": predictions_list,
        "main_prediction": predictions_list[0]["disease"],
    }

if __name__ == "__main__":
    import uvicorn
    uvicorn.run(app, host="0.0.0.0", port=8000)