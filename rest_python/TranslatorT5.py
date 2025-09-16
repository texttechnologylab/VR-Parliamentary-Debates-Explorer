from transformers import T5Tokenizer, T5ForConditionalGeneration
import torch


class Translator:
    def __init__(self, model_name: str = "google/flan-t5-base"):
        self.tokenizer = T5Tokenizer.from_pretrained(model_name)
        if torch.cuda.is_available():
            self.model = T5ForConditionalGeneration.from_pretrained(model_name, device_map="auto")
        else:
            self.model = T5ForConditionalGeneration.from_pretrained(model_name)

    def translate(self, sentence: str, language1: str = "English", language2: str = "German", max_source_length: int = 512):
        # English to german

        pre_sentence = f"translate {language1} to {language2}: {sentence}"
        if torch.cuda.is_available():
            input_ids = self.tokenizer(pre_sentence, return_tensors="pt", padding="longest").input_ids.to("cuda")
        else:
            input_ids = self.tokenizer(pre_sentence, return_tensors="pt", padding="longest").input_ids
        outputs = self.model.generate(input_ids, max_length=max_source_length)
        return self.tokenizer.decode(outputs[0], skip_special_tokens=True)


if __name__ == '__main__':
    input_text = "A chatbot is an artificial intelligence system that is designed to simulate human conversation and interact with users through text or voice. Chatbots use natural language processing, machine learning and other advanced technologies to understand user requests and generate responses. They are typically used in customer service scenarios to provide 24/7 support and improve the customer experience."
    translator_model = Translator("google/flan-t5-base")
    print(translator_model.translate(input_text))
