package main
import (
	"fmt"
	"io"
	"log"
	"net/http"
	"os"
)
func uploadHandler(w http.ResponseWriter, r *http.Request) {
	//Limita il file a 10MB
	//r.ParseMultipartForm(10 << 20)
	//Recupera il documento dal form con la chiave "file"
	file, handler, err := r.FormFile("file")
	if err != nil {
		http.Error(w, "Errore nel recupero del documento", http.StatusBadRequest)
		return
	}
	defer file.Close()
	//fmt.Printf("Ricevuto documento: %s (%d bytes)\n", handler.Filename, handler.Size)
	dstPath := "./uploads"
	//Crea la cartella di destinazione se non esiste
	os.MkdirAll(dstPath, os.ModePerm)
	/* Costruisco il percorso completo del file di destinazione
	"./uploads/nomefile.ext"
	string(os.PathSeparator) gestisce il separatore di percorso in base al sistema operativo
	"/" su Linux e "\" su Windows */
	fullPath := dstPath + string(os.PathSeparator) + handler.Filename
	//Crea il file nella cartella di destinazione
	dst, err := os.Create(fullPath)
	if err != nil {
		http.Error(w, "Errore nel salvataggio", http.StatusInternalServerError)
		return
	}
	defer dst.Close()
	//Copia il contenuto del file caricato nel file di destinazione a blocchi di 32KB
	buffer := make([]byte, 32000) 
	var written int64
	var i int
	i=0;
	for {
		n, readErr := file.Read(buffer)
		if n > 0 {
			x, writeErr := dst.Write(buffer[:n])
			//fmt.Printf("Letti %d byte, Scritti %d byte, Iterazione %d\n", n, x, i)
			if writeErr != nil || x != n {
				http.Error(w, "Errore in scrittura", 500)
				return
			}
			written += int64(x)
		}
		if readErr == io.EOF {
			break
		}
		if readErr != nil {
			http.Error(w, "Errore in lettura", 500)
			return
		}
		i++;
	}
	//fmt.Printf("Copiati %d byte\n", written)
	//fmt.Printf("Salvato documento: %s (%d bytes su %d bytes)\n", fullPath, written, handler.Size)
	go startPythonAnalysis(fullPath)
	//fmt.Fprintf(w, "File caricato correttamente: %s", handler.Filename)
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