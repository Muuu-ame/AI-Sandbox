from fastapi import FastAPI
from fastapi.middleware.cors import CORSMiddleware

# ==========================================
# FastAPI Application
# ==========================================

app = FastAPI(
    title="AI Sandbox API",
    description="AI Sandbox communication server",
    version="0.2.0"
)

# ==========================================
# CORS Settings
# ==========================================

app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"],
    allow_credentials=False,
    allow_methods=["*"],
    allow_headers=["*"],
)

# ==========================================
# Root Endpoint
# ==========================================

@app.get("/")
def root():
    return {
        "message": "AI Sandbox Server is running!",
        "status": "OK"
    }

# ==========================================
# Chat Endpoint
# ==========================================

@app.post("/chat")
def chat(data: dict):

    # Unityから受信したメッセージ
    user_message = data.get("message", "")

    print(f"[Unity] Received message: {user_message}")

    # 現段階では固定の命令を返す
    command = {
        "command": "create_object",
        "parameters": {
            "shape": "cube",
            "position": [0, 1, 3],
            "scale": [2, 2, 2],
            "color": "red"
        }
    }

    return command