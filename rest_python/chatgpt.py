import openai
import json

class ChatGpt:
    def __init__(self, api_key: str, model_engine: str = "gpt-3.5-turbo"):
        openai.api_key = api_key
        self.model_engine = model_engine

    def get_response(self, question: str):
        completion = openai.ChatCompletion.create(
            model=self.model_engine,
            messages=[{"role": "user", "content": question}],
            max_tokens=3000,
            n=1,
            stop=None,
            temperature=0.5,
        )
        return completion.choices[0].message.content


if __name__ == '__main__':
    with open("data/config_chatgpt.json", "r", encoding="UTF-8") as json_file:
        key_chatgpt = json.load(json_file)
    chatgpt_text = ChatGpt(key_chatgpt["key"])
    print(chatgpt_text.get_response("What is a Chatbot"))