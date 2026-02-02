package main

import (
	"fmt"
	"io"
	"log"
	"net/http"
	"os"
	"path/filepath"
)

func uploadHandler(w http.ResponseWriter, r *http.Request) {
	// 1. Limita la dimensione del caricamento (es. 10 MB)
	r.ParseMultipartForm(10 << 20)

	// 2. Recupera il file dal form (la chiave deve corrispondere a quella in C#)
	file, handler, err := r.FormFile("file")
	if err != nil {
		http.Error(w, "Errore nel recupero del file", http.StatusBadRequest)
		return
	}
	defer file.Close()

	fmt.Printf("Ricevuto file: %s\n", handler.Filename)

	// 3. Crea la cartella di destinazione se non esiste
	dstPath := "./uploads"
	os.MkdirAll(dstPath, os.ModePerm)

	// 4. Crea il file fisico sul disco
	fullPath := filepath.Join(dstPath, handler.Filename)
	dst, err := os.Create(fullPath)
	if err != nil {
		http.Error(w, "Errore nel salvataggio", http.StatusInternalServerError)
		return
	}
	defer dst.Close()

	// 5. Copia il contenuto del file caricato nel file di destinazione
	if _, err := io.Copy(dst, file); err != nil {
		http.Error(w, err.Error(), http.StatusInternalServerError)
		return
	}

	// 6. QUI chiamerai il terzo processo (Python)
	go startPythonAnalysis(fullPath)

	fmt.Fprintf(w, "File caricato correttamente: %s", handler.Filename)
}

func startPythonAnalysis(filePath string) {
	// Per ora stampiamo solo un messaggio. 
	// Qui userai il pacchetto "os/exec" per lanciare Python.
	fmt.Printf("Analizzatore avviato per: %s\n", filePath)
}

func main() {
	http.HandleFunc("/upload", uploadHandler)
	fmt.Println("Server Go avviato su http://localhost:8080")
	if err := http.ListenAndServe(":8080", nil); err != nil {
		log.Fatal(err)
	}
}