using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Programozasi_kornyezetek_ZH {
	/*
1. Készíts osztályt, Játék néven! Adattagok: folyamatban (logikai), maximális pontszám, játékosok száma, játékosok pontszáma. 
Minden játékosnak nyilván tartjuk a pontszámát, a játékosok számának nincs felső korlátja, mindenki 0 ponttól indul. 
Készíts metódust PontSzerzés néven, ami a paraméterben megadott számú játékos pontszámát növeli a paraméterben megadott mértékben!
Az osztály saját eseménye jelezze, ha valaki elérte a pontszámot és azt is, hogy melyik számú játékos!

2. Készíts gyűjteményt legalább 5 játékból, a játékok elérése kulcsszó alapján (játék neve) történjen! Írasd ki a folyamatban 
lévő játékokban a játékosok számát és összpontszámukat!

3. Készíts az előző gyűjtemény alapján LINQ lekérdezéseket!
- Hány játék van befejezve (nincs folyamatban)?
- Mennyi az átlagos játékosszám?
- A folyamatban lévő lévő játékok maximális pontszámai
4. Bináris adatfájlba mentsük valamennyi játék adatait a gyűjteményből!
	 */

	class Játék {
		class PontszamFigyeles {
			private int felsoHatar;
			private int maximalisPontszam;

			public PontszamFigyeles(int passedFelsoHatar) {
				felsoHatar = passedFelsoHatar;
			}

			public void Add(int x) {
				maximalisPontszam = x;
				if (maximalisPontszam >= felsoHatar) {
					OnThresholdReached(EventArgs.Empty);
				}
			}

			protected virtual void OnThresholdReached(EventArgs e) {
				EventHandler handler = ThresholdReached;
				if (handler != null) {
					handler(this, e);
				}
			}

			public event EventHandler ThresholdReached;
		}

		private string jatekNeve;
		private bool folyamatban;
		private int maximalisPontszam;
		private int jatekosokSzama;
		Dictionary<int, int> jatekosokPontszama = new Dictionary<int, int>();

		public Játék() {
		}

		public Játék(int maximalisPontszam, int jatekosokSzama) {
			this.maximalisPontszam = maximalisPontszam;
			this.jatekosokSzama = jatekosokSzama;
			for (int i = 0; i < this.jatekosokSzama; i++) {
				jatekosokPontszama.Add(i, 0);
			}
		}

		public Játék(bool folyamatban, int maximalisPontszam, int jatekosokSzama) {
			this.folyamatban = folyamatban;
			this.maximalisPontszam = maximalisPontszam;
			this.jatekosokSzama = jatekosokSzama;
			for (int i = 0; i < this.jatekosokSzama; i++) {
				jatekosokPontszama.Add(i, 0);
			}
		}

		public Játék(string jatekNeve, bool folyamatban, int maximalisPontszam, int jatekosokSzama) {
			this.jatekNeve = jatekNeve;
			this.folyamatban = folyamatban;
			this.maximalisPontszam = maximalisPontszam;
			this.jatekosokSzama = jatekosokSzama;
			for (int i = 0; i < this.jatekosokSzama; i++) {
				jatekosokPontszama.Add(i, 0);
			}
		}

		public string JatekNeve {
			get => jatekNeve;
			set => jatekNeve = value;
		}

		public bool Folyamatban {
			get => folyamatban;
			set => folyamatban = value;
		}

		public int MaximalisPontszam {
			get => maximalisPontszam;
			set => maximalisPontszam = value;
		}

		public int JatekosokSzama {
			get => jatekosokSzama;
			set => jatekosokSzama = value;
		}

		public Dictionary<int, int> JatekosokPontszama {
			get => jatekosokPontszama;
			set => jatekosokPontszama = value;
		}

		public void PontSzerzés(int jatekosSzama, int pontszam) {
			if (jatekosokPontszama.ContainsKey(jatekosSzama)) {
				jatekosokPontszama[jatekosSzama] += pontszam;
			}

			PontszamFigyeles pontszamFigyeles = new PontszamFigyeles(maximalisPontszam);
			pontszamFigyeles.ThresholdReached += pontszamElerve;
			pontszamFigyeles.Add(FindMaximalisPontszamuJatekos().Item2);
		}
		
		

		public Tuple<int, int> FindMaximalisPontszamuJatekos() {
			int jatekosSzama = 0;
			int maxPontszam = jatekosokPontszama[0];
			foreach (var jp in jatekosokPontszama) {
				if (maxPontszam < jp.Value) {
					jatekosSzama = jp.Key;
					maxPontszam = jp.Value;
				}
			}

			return new Tuple<int, int>(jatekosSzama, maxPontszam);;
		}

		void pontszamElerve(object sender, EventArgs e) {
			var jatekos = FindMaximalisPontszamuJatekos();
			Console.WriteLine("Pontszam elerve!" + " Játékos: " + jatekos.Item1 + " Pontszám: " + jatekos.Item2);
		}

		public int OsszPontszam() {
			int sum = 0;
			foreach (var pontszam in jatekosokPontszama) {
				sum += pontszam.Value;
			}

			return sum;
		}
		
		
	}
	
	

	internal class Program {
		public static void ToBinaryFile(string? path, string data) {
			path ??= "../../../binaryfile.dat";
			using (BinaryWriter writer = new BinaryWriter(File.Open(path, FileMode.Create))) {
				writer.Write(data + "\n");
			}
		}
		
		public static void Main(string[] args) {
			var jatekok = new Dictionary<string, Játék>();
			Játék jatek1 = new Játék();

			for (int i = 0; i < 5; i++) {
				Random random = new Random();
				int jatekosokSzama = random.Next(2, 10);
				bool folyamatban = (random.Next(50) <= 25) ? true : false;
				Játék játék = new Játék(i.ToString(),folyamatban, random.Next(10, 50), jatekosokSzama);
				játék.PontSzerzés(random.Next(1, jatekosokSzama - 1), random.Next(1, 100));
				jatekok.Add(i.ToString(), játék);
			}

			foreach (var jatek in jatekok) {
				Console.WriteLine("Jatek neve: " + jatek.Key + ", Jatekosok szama: " + jatek.Value.JatekosokSzama +
				                  ", Osszpontszam: " + jatek.Value.OsszPontszam() + " Folyamatban: " + (jatek.Value.Folyamatban?"igen":"nem"));
			}
			
			var folyamatbanLevoJatekok = jatekok.Values.Where(x => !x.Folyamatban ).ToList();
			Console.WriteLine("Befejezett jatekok:");
			foreach (var j in folyamatbanLevoJatekok) {
				Console.WriteLine("Neve: " + j.JatekNeve + " Osszpontszam: " + j.OsszPontszam() + " Folyamatban: " + (j.Folyamatban?"igen":"nem"));
			}
			
			var atlagJatekosszam = jatekok.Values.Average(x => x.JatekosokSzama);
			Console.WriteLine("Atlag jatekosszam> " + atlagJatekosszam);

			var folyamatbanLevoJatekokPontszamai = jatekok.Values.Where(x => x.Folyamatban ).ToList();
			Console.WriteLine("A folyamatban lévő lévő játékok maximális pontszámai:");
			foreach (var f in folyamatbanLevoJatekokPontszamai) {
				Console.WriteLine(f.JatekNeve + " : " + f.MaximalisPontszam);
			}
			
			string data = "";
			foreach (var jatek in jatekok) {
				data += jatek.Value.JatekNeve + " " + jatek.Value.JatekosokPontszama + " " + jatek.Value.JatekosokSzama + " " +
				        jatek.Value.MaximalisPontszam + " " + jatek.Value.Folyamatban + "\n";
			}
			ToBinaryFile("binary.dat", data);
		}
	}
}