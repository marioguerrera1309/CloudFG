import sys
import os
import spacy
import requests
import hashlib
import json
import fitz  # PyMuPDF
from docx import Document as DocxReader
try:
    nlp = spacy.load("it_core_news_sm")
except OSError:
    print("Errore: Modello 'it_core_news_sm' non trovato.")
    sys.exit(1)
    
#funzione per calcolare l'hash del file
def get_file_hash(file_path):
    hash = hashlib.sha256()
    with open(file_path, "rb") as f:
        for byte_block in iter(lambda: f.read(4096), b""):
            hash.update(byte_block)
    return hash.hexdigest()

def extract_text(file_path):
    ext = os.path.splitext(file_path)[1].lower()
    testo = ""

    if ext == ".txt":
        with open(file_path, 'r', encoding='utf-8') as f:
            testo = f.read()
            
    elif ext == ".pdf":
        doc = fitz.open(file_path)
        for pagina in doc:
            testo += pagina.get_text()
        doc.close()
        
    elif ext == ".docx":
        doc = DocxReader(file_path)
        testo = "\n".join([para.text for para in doc.paragraphs])
        
    else:
        raise ValueError(f"Formato {ext} non supportato per l'analisi del testo")
    
    return testo


def main():
    if len(sys.argv) > 1:
        file_path = sys.argv[1]
        print(f"Python ha ricevuto: {file_path}")
    else:
        print("Nessun parametro ricevuto")
        sys.exit(1)
    try:
        hash = get_file_hash(file_path)
        testo = extract_text(file_path)
    except Exception as e:
        print(f"Errore calcolo hash: {e}")
        sys.exit(1)
    
    doc = nlp(testo)
    n_lettere = sum(len(token.text) for token in doc if token.is_alpha)
    n_parole = len([token for token in doc if not token.is_punct])
    n_frasi = len(list(doc.sents))
    if n_parole == 0:
        indice = 0
    else:
        WPM=200 #numero di parole lette al minuto secondo le statistiche
        # Formula Gulpease: 89 + (300 * frasi - 10 * lettere) / parole
        indice = 89 + (300 * n_frasi - 10 * n_lettere) / n_parole
        tempo_lettura_minuti = n_parole /WPM 
    # Creazione del json da inviare al server Go
    analitics = {
        "file_path": file_path,
        "gulpease_index": indice,
        "letters": n_lettere,
        "words": n_parole,
        "sentences": n_frasi,
        "read_time": tempo_lettura_minuti
    }
    data = {
        "hash": hash,
        "analitics": analitics
    }
    url = "http://localhost:8080/upload_analitics"
    try:
        response = requests.post(url, json=data)
        if response.status_code != 200:
            print(f"Server Go ha risposto con errore: {response.status_code}")
    except Exception as e:
        print(f"Errore durante l'invio al server Go: {e}")

if __name__ == "__main__":
    main()