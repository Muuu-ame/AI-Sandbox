
from fastapi import FastAPI

app = FastAPI()


@app.get("/")
async def root():
    return {"message": "AI Sandbox API is running"}


@app.post("/command")
async def receive_command(data: dict):
    print("Unityから受信:")
    print(data)

    return {
        "commands": [],
        "message": "Pythonからの応答です"
    }
