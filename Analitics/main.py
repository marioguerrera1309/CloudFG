import sys
import spacy

try:
    nlp = spacy.load("it_core_news_sm")
except OSError:
    # Fallback se il modello non è scaricato
    print("Errore: Modello 'it_core_news_sm' non trovato.")
    sys.exit(1)

def main():
    if len(sys.argv)>1:
        file_path=sys.argv[1]
        print(f"Python ha ricevuto: {file_path}")
    else:
        print("Nessun parametro ricevuto")
    
    try:
        with open(file_path, 'r', encoding='utf-8') as f:
            testo = f.read()
    except Exception as e:
        return f"Errore durante la lettura del file: {e}"
    
    doc = nlp(testo)

    # 1. Conteggio Lettere (solo caratteri alfanumerici)
    # spaCy identifica ogni token; noi contiamo solo i caratteri delle parole
    n_lettere = sum(len(token.text) for token in doc if token.is_alpha)
    
    # 2. Conteggio Parole
    # Escludiamo la punteggiatura dal conteggio delle parole
    n_parole = len([token for token in doc if not token.is_punct])
    
    # 3. Conteggio Frasi
    # spaCy usa il parsing sintattico per capire dove finisce una frase
    n_frasi = len(list(doc.sents))

    if n_parole == 0:
        return 0

    # Formula Gulpease: 89 + (300 * frasi - 10 * lettere) / parole
    indice = 89 + (300 * n_frasi - 10 * n_lettere) / n_parole
    print(indice)
if __name__ == "__main__":
    main()