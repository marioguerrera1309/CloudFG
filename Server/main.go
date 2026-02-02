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
	//Limita il file a 10MB
	r.ParseMultipartForm(10 << 20)
	file, handler, err := r.FormFile("file")
	if err != nil {
		http.Error(w, "Errore nel recupero del file", http.StatusBadRequest)
		return
	}
	defer file.Close()
	//fmt.Printf("Ricevuto file: %s (%d bytes)\n", handler.Filename, handler.Size)
	dstPath := "./uploads"
	//Crea la cartella di destinazione se non esiste
	os.MkdirAll(dstPath, os.ModePerm)
	//filepath.join unisce il percorso della cartella con il nome del file
	fullPath := filepath.Join(dstPath, handler.Filename)
	//Crea il file nella cartella di destinazione
	dst, err := os.Create(fullPath)
	if err != nil {
		http.Error(w, "Errore nel salvataggio", http.StatusInternalServerError)
		return
	}
	defer dst.Close()
	//Copia il contenuto del file caricato nel file di destinazione a blocchi di 32KB
	written, err := io.Copy(dst, file)
	if err != nil {
		http.Error(w, err.Error(), http.StatusInternalServerError)
		return
	}
	fmt.Printf("Salvato file: %s (%d bytes su %d bytes)\n", fullPath, written, handler.Size)
	go startPythonAnalysis(fullPath)
	fmt.Fprintf(w, "File caricato correttamente: %s", handler.Filename)
}
func startPythonAnalysis(filePath string) {
	fmt.Printf("Analizzatore avviato per: %s\n", filePath)
}
func main() {
	http.HandleFunc("/upload", uploadHandler)
	fmt.Println("Server Go avviato su http://localhost:8080")
	if err := http.ListenAndServe(":8080", nil); err != nil {
		log.Fatal(err)
	}
}