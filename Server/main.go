package main
import (
	"fmt"
	"runtime"
)
func main() {
	fmt.Println("=== TEST AMBIENTE GO ===")
	fmt.Printf("OS: %s\nArchitettura: %s\n", runtime.GOOS, runtime.GOARCH)
	fmt.Print("\nCome ti chiami? ")
	var nome string
	fmt.Scanln(&nome)
	if nome != "" {
		fmt.Printf("Ciao %s, Go funziona perfettamente!\n", nome)
	} else {
		fmt.Println("Non hai inserito un nome, ma il test è comunque passato.")
	}
}