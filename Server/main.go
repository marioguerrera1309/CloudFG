package main
import (
	"crypto/sha256"
	"database/sql"
	"fmt"
	"log"
	"net/http"
	"os"
	_"modernc.org/sqlite"
)
var db *sql.DB
func initDatabase() {
	var err error
	// Apre (o crea) il file libgen.db. Nota il nome del driver "sqlite"
	db, err = sql.Open("sqlite", "./libgen.db")
	if err != nil {
		log.Fatal("Errore apertura DB:", err)
	}
	// Creazione della tabella per i file e i loro metadati
	statement := `
    CREATE TABLE IF NOT EXISTS files (
        hash TEXT PRIMARY KEY,
        title TEXT,
        author TEXT,
        size_bytes INTEGER,
        file_path TEXT
    );`
	_, err = db.Exec(statement)
	if err != nil {
		log.Fatal("Errore creazione tabella:", err)
	}
	fmt.Println("Database pronto!")
}
func uploadHandler(w http.ResponseWriter, r *http.Request) {
	//Limita il file a 10MB
	//r.ParseMultipartForm(10 << 20)
	//Recupera il documento dal form con la chiave "file"
	file, handler, err := r.FormFile("file")
	title := r.FormValue("title")
	author := r.FormValue("author")
	if err != nil {
		http.Error(w, "Errore nel recupero del documento", http.StatusBadRequest)
		return
	}
	defer file.Close()
	hasher := sha256.New()
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
	//con defer chiude il file di destinazione alla fine della funzione
	defer dst.Close()
	//Copia il contenuto del file caricato nel file di destinazione a blocchi di 32KB
	var buffer = make([]byte, 32*1024) //Buffer di 32KB
	var written int64
	var i int
	i = 0
	for {
		n, readErr := file.Read(buffer)
		if n > 0 {
			x, writeErr := dst.Write(buffer[:n])
			//fmt.Printf("Letti %d byte, Scritti %d byte, Iterazione %d\n", n, x, i)
			if writeErr != nil || x != n {
				http.Error(w, "Errore in scrittura", 500)
				return
			}
			hasher.Write(buffer[:n])
			written += int64(x)
		}
		if readErr != nil {
			if readErr.Error() == "EOF" {
				break
			}
			http.Error(w, "Errore in lettura", 500)
			return
		}
		i++
	}
	//fmt.Printf("Copiati %d byte\n", written)
	fileHash := fmt.Sprintf("%x", hasher.Sum(nil))
	var exists bool
	err = db.QueryRow("SELECT EXISTS(SELECT 1 FROM files WHERE hash=?)", fileHash).Scan(&exists)
	if exists {
		fmt.Printf("Duplicato rilevato! Hash: %s\n", fileHash)
		http.Error(w, "File già esistente nel database", http.StatusConflict) // Codice 409
		return
	}
	_, err = db.Exec("INSERT INTO files (hash, title, author, size_bytes, file_path) VALUES (?, ?, ?, ?, ?)",
		fileHash, title, author, written, fullPath)
	if err != nil {
		http.Error(w, "Errore registrazione DB", http.StatusInternalServerError)
		return
	}
	fmt.Printf("Salvato: %s | Author: %s | Hash: %s | Bytes: %d | Path: %s \n", title, author, fileHash, written, fullPath)
	w.WriteHeader(http.StatusOK)
	fmt.Fprintf(w, "File caricato con successo!")
	go startPythonAnalysis(fullPath)
	//fmt.Fprintf(w, "File caricato correttamente: %s", handler.Filename)
}
func startPythonAnalysis(filePath string) {
	fmt.Printf("Analizzatore avviato per: %s\n", filePath)
}
func main() {
	initDatabase()
	http.HandleFunc("/upload", uploadHandler)
	fmt.Println("Server Go avviato su http://localhost:8080")
	if err := http.ListenAndServe(":8080", nil); err != nil {
		fmt.Printf("Errore nell'avvio del server: %s\n", err)
	}
}
