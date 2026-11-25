from fastapi import FastAPI, HTTPException
from fastapi.middleware.cors import CORSMiddleware
from pydantic import BaseModel
import ollama
import json


# ==========================================
# FastAPI Application
# ==========================================

app = FastAPI(
    title="AI Sandbox API",
    description="AI Sandbox communication server",
    version="0.3.0"
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
# Request Model
# ==========================================

class ChatRequest(BaseModel):
    message: str


# ==========================================
# AI Settings
# ==========================================

MODEL_NAME = "qwen3:8b"

SYSTEM_PROMPT = """
あなたは3Dゲームの世界を操作するAIです。

ユーザーの自然言語による指示を解釈し、
必ずJSON形式だけで返してください。

説明文、Markdown、コードブロックは出力しないでください。

使用できる命令は以下です。

1. create_object
オブジェクトを生成する

2. set_gravity
重力を変更する

3. set_scale
オブジェクトの大きさを変更する

4. move_object
オブジェクトを移動する

JSON形式は以下に従ってください。

オブジェクト生成の場合：
{
    "command": "create_object",
    "parameters": {
        "shape": "cube",
        "position": [0, 1, 3],
        "scale": [2, 2, 2],
        "color": "red"
    }
}

重力変更の場合：
{
    "command": "set_gravity",
    "parameters": {
        "value": -4.9
    }
}

大きさ変更の場合：
{
    "command": "set_scale",
    "parameters": {
        "target": "箱",
        "scale": [3, 3, 3]
    }
}

移動の場合：
{
    "command": "move_object",
    "parameters": {
        "target": "箱",
        "offset": [1, 0, 0]
    }
}

判断できない場合や対応できない指示の場合：
{
    "command": "unknown",
    "parameters": {}
}

必ずJSONとして正しく解釈できる形式で返してください。
"""


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
def chat(data: ChatRequest):

    user_message = data.message

    print(f"[Unity] Received message: {user_message}")

    try:
        response = ollama.chat(
            model=MODEL_NAME,
            messages=[
                {
                    "role": "system",
                    "content": SYSTEM_PROMPT
                },
                {
                    "role": "user",
                    "content": user_message
                }
            ],
            format={
                "type": "object",
                "properties": {
                    "command": {
                        "type": "string"
                    },
                    "parameters": {
                        "type": "object"
                    }
                },
                "required": ["command", "parameters"]
            },
            options={
            "temperature": 0
            }
        )

        ai_response = response["message"]["content"]

        print(f"[Ollama] Response: {ai_response}")

        # JSONとして解析
        command_data = json.loads(ai_response)

        return command_data

    except json.JSONDecodeError:
        raise HTTPException(
            status_code=500,
            detail="AI returned invalid JSON"
        )

    except Exception as e:
        print(f"[ERROR] {e}")

        raise HTTPException(
            status_code=500,
            detail=str(e)
        )
