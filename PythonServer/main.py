from fastapi import FastAPI, HTTPException
from fastapi.middleware.cors import CORSMiddleware
from pydantic import BaseModel, Field
import ollama
import json

app = FastAPI(
    title="AI Sandbox API",
    description="AI Sandbox communication server",
    version="0.4.0"
)

app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"],
    allow_credentials=False,
    allow_methods=["*"],
    allow_headers=["*"],
)

class ChatRequest(BaseModel):
    message: str
    objects: list[dict] = Field(default_factory=list)


MODEL_NAME = "qwen3:8b"


SYSTEM_PROMPT = """
あなたは3Dゲームの世界を操作するAIです。

ユーザーの自然言語による指示を解釈し、
必ずJSON形式だけで返してください。

説明文、Markdown、コードブロックは禁止です。

使用できる命令:

1. create_object
Cubeを生成する。

{
  "command": "create_object",
  "parameters": {
    "shape": "cube",
    "position": [0, 1, 3],
    "scale": [2, 2, 2],
    "color": "#FF0000"
  }
}

2. delete_object
指定されたオブジェクトを削除する。

{
  "command": "delete_object",
  "parameters": {
    "target": "対象のオブジェクトID"
  }
}

削除対象は必ず現在のワールド情報から選んでください。

存在しないIDを作ってはいけません。
対象を一意に特定できない場合はunknownを返してください。

3. unknown
対応できない命令。

{
  "command": "unknown",
  "parameters": {}
}

必ずJSONだけを返してください。
"""


@app.get("/")
def root():
    return {
        "message": "AI Sandbox Server is running!",
        "status": "OK"
    }


@app.post("/chat")
def chat(data: ChatRequest):
    print(f"[Unity] Received message: {data.message}")
    print(f"[Unity] World objects: {len(data.objects)}")

    try:
        world_info = json.dumps(
            data.objects,
            ensure_ascii=False
        )

        response = ollama.chat(
            model=MODEL_NAME,
            messages=[
                {
                    "role": "system",
                    "content": SYSTEM_PROMPT
                },
                {
                    "role": "user",
                    "content": (
                        "現在のワールド情報:\n"
                        + world_info
                        + "\n\nユーザーの指示:\n"
                        + data.message
                    )
                }
            ],
            format={
                "type": "object",
                "properties": {
                    "command": {
                        "type": "string",
                        "enum": [
                            "create_object",
                            "delete_object",
                            "unknown"
                        ]
                    },
                    "parameters": {
                        "type": "object"
                    }
                },
                "required": [
                    "command",
                    "parameters"
                ]
            },
            options={
                "temperature": 0
            }
        )

        ai_response = response["message"]["content"]

        print("[Ollama Raw Response]")
        print(repr(ai_response))

        command_data = json.loads(ai_response)

        # 削除対象IDの検証
        if command_data["command"] == "delete_object":
            target = command_data["parameters"].get("target")

            valid_ids = {
                obj.get("id")
                for obj in data.objects
                if obj.get("id")
            }

            if target not in valid_ids:
                command_data = {
                    "command": "unknown",
                    "parameters": {}
                }

        print("[Ollama] Response:", command_data)

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
