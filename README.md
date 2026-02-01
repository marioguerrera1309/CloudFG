1. Requisiti Fondamentali
Prima di aprire VS Code, devi installare il "cuore" di .NET:
1.	Scarica .NET SDK: Vai sul sito ufficiale di Microsoft e scarica l'ultima versione del .NET SDK (attualmente .NET 8 o .NET 9).
2.	Verifica l'installazione: Apri un terminale (Prompt dei comandi o PowerShell) e digita:
dotnet --version
Se vedi un numero di versione, sei pronto a partire.
________________________________________
2. Configura Visual Studio Code
Apri VS Code e installa l'estensione indispensabile:
•	Vai nella sezione Extensions (l'icona dei quadratini a sinistra).
•	Cerca e installa "C# Dev Kit" (sviluppata da Microsoft). Questa estensione installa automaticamente tutto il necessario, incluso il supporto per il debugger e la gestione dei progetti.
________________________________________
3. Creare e Compilare un Progetto (Metodo Terminale)
Il modo più veloce e professionale per compilare è usare il terminale integrato di VS Code (Ct
1.	Crea una cartella per il tuo progetto e aprila in VS Code. 
2.	Inizializza il progetto: Nel terminale spostati sulla cartella del progetto e scrivi:
dotnet new console
(Questo creerà un file Program.cs con un classico "Hello World")
3.	Compila il progetto:
dotnet build
(Questo verifica che non ci siano errori e crea i file binari)
4.	Esegui il programma:
dotnet run
________________________________________
4. Compilazione ed Esecuzione
Esistono due modi principali per far girare il codice:
Metodo B: Tramite Terminale (Per capire cosa succede)
Se vuoi farlo "alla vecchia maniera" per avere più controllo:
1.	Apri il terminale di VS Code e spostati nella cartella del tuo progetto.
2.	Compila il file scrivendo:
g++ main.cpp -o mio_programma.exe
3.	Eseguilo scrivendo:
./mio_programma.exe

PYTHON
1)Apri il terminale e spostati nella cartella dove si trova il file.py
2)Attiva wsl
3) scrivi python3 nome_file.py per eseguire nome_file.py

GO
1. Preparazione dell'ambiente
1.	Scarica Go: Vai su go.dev/dl e scarica l'installer per Windows (.msi).
2.	Verifica: Apri il terminale e scrivi go version. Se risponde, il "motore" è pronto.
3.	Estensione VS Code: Cerca su visual studio l'estensione "Go" (quella ufficiale del team Go/Google) e installala.
2. Creazione del progetto (Il modulo)
A differenza di Python, in Go ogni progetto deve essere inizializzato come un "modulo".
1.	Crea una cartella per il tuo progetto e aprila in VS Code.
2.	Apri il terminale integrato (Ctrl + `), spostati sulla cartella del progetto e scrivi:
go mod init nome-progetto
(Questo creerà un file go.mod che serve a gestire le dipendenze).
3. Scrivere il codice di test
Crea un file chiamato main.go, inseriscilo all’interno di nome-progetto e incolla questo codice:
package main

import (
	"fmt"
	"runtime"
)

func main() {
	fmt.Println("=== TEST AMBIENTE GO ===")
	
	// Verifica versione e OS
	fmt.Printf("OS: %s\nArchitettura: %s\n", runtime.GOOS, runtime.GOARCH)

	// Test Input
	fmt.Print("\nCome ti chiami? ")
	var nome string
	fmt.Scanln(&nome)

	if nome != "" {
		fmt.Printf("Ciao %s, Go funziona perfettamente!\n", nome)
	} else {
		fmt.Println("Non hai inserito un nome, ma il test è comunque passato.")
	}
}
4. I comandi principali per eseguire e compilare
In Go non usi quasi mai tasti "Play" complessi; si fa tutto da terminale perché è istantaneo.
•	Eseguire senza creare file extra: Se vuoi solo testare il codice velocemente senza generare un file .exe:
Bash
go run main.go
•	Compilare (Creare l'eseguibile): Per trasformare il tuo codice in un file test-progetto.exe pronto da usare:
Bash
go build
•	Installare le utility di VS Code: La prima volta che apri un file .go, VS Code ti chiederà in basso a destra: "The 'gopls' command is not available". Clicca su Install All. Questo scaricherà gli strumenti per l'autocompletamento e il debug.


