using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace Chasse_au_trésor
{
  class Program
  {
    /*================================================================================================================================
    *                                                      FONCTION PRINCIPALE
    ================================================================================================================================*/
    static void Main(string[] args)
    {
      //-- Variables --

      string motifName;
      bool continuer = true;

      //-- Ecran d'accueil et choix du menu --
      string MenuChoisi = AfficherEcranAccueil();
          //-- Stats --
      while (MenuChoisi == "stat")
      {
        AfficherStatistiquesDeJeu();
        var Touche = Console.ReadKey().Key;
        while (Touche != ConsoleKey.Escape)
        {
          Touche = Console.ReadKey().Key;
        }
        MenuChoisi = AfficherEcranAccueil();
        Console.WriteLine(MenuChoisi);
      }
          //-- Jeu --
      if (MenuChoisi == "jeu")
      {
        //-- Gestion username --
        GererUsername();
        //-- Gestion versionning --
        TesterVersionning();//Rajouter etat
        

        //-- Affichage des règles seulement si elles n'ont jamais été visionnées --
        if (!System.IO.File.Exists(@"ScoreSaver.txt"))
        {
          AfficherRegle();
        }
        Console.Clear();

        //-- Affichage intro --
        AfficherIntro();

        //-- Session de jeu --
        while (continuer)
        {
          string[] etatsTableau = File.ReadAllLines(@"etatsSaver.txt");
          string etat1 = etatsTableau[0];
          string etat2 = etatsTableau[1];
          string etat3 = etatsTableau[2];
          string etat4 = etatsTableau[3];
          //-- Choix de sauvegarde et de son utilisation (Charger ou Recommencer) --
          AfficherEtatSauvegarde2(etat1, etat2, etat3, etat4);
          string nb = ChoisirNumeroSave();
          string choix = ChoisirRecommencerOuCharger();
          string txt = "save" + nb + ".txt";
          //-- Recommencer --
          if (choix == "Recommencer")
          {
            motifName = ChoisirTriangleOuRectangle();
            EcrireMotifsSauvegarde(int.Parse(nb), motifName);
          }
          else
          {
            //-- Charger --
            string[] motifs = File.ReadAllLines(@"motifsSave.txt");
            motifName = motifs[int.Parse(nb) - 1];
          }
          //-- Récupération des informations --
          string[][] grilleUU = GererVersionning(choix, @txt, ref etatsTableau[int.Parse(nb) - 1], etatsTableau);
          string[] informationsUtiles = ExtraireCodeUtile(grilleUU);
          string[][] grille = ExtraireGrille(grilleUU);


          int nbb = int.Parse(nb) - 1;
          Jouer(ref etatsTableau[int.Parse(nb) - 1], @txt, grille, motifName, etatsTableau, choix, nbb);
          System.Threading.Thread.Sleep(1500);
          Console.Clear();
          //-- Contnuer ou non de jouer --
          continuer = ChoisirRefairePartie();
          Console.Clear();
        }
      }
    }

    static public void affichage_test(string[][] grille)
    {
      for (int i = 0; i < grille.Length; i++)
      {
        for (int j = 0; j < grille[1].Length; j++)
        {
          Console.Write(grille[i][j] + "  ");

        }
        Console.WriteLine();
      }
    }
    static public void Jouer(ref string etat, string saveTxt, string[][] grille, string motifName, string[] etatsTab, string choix, int nb)
    {
      //--- Déclarations ---
      Random rng = new Random();
      int score = 1;
      bool perdu = false;
      int hauteur = grille.Length;
      int largeur = grille[1].Length;
      string ht;
      bool success;
      int findx;
      int findy;
      int nbMines;
      int nbTresor;
      int nbCases;
      string[][] tab = CreerRectangle(20, 20);

      //--- Recommencer ---
      if (choix == "Recommencer")
      {
        //--- Motif Rectangle ---
        if (motifName == "rectangle")
        {
          Console.WriteLine("Veuillez saisir la longueur de votre rectangle");
          ht = Console.ReadLine();
          success = int.TryParse(ht, out hauteur);
          while (hauteur <= 0 || success!=true)
          {
            Console.WriteLine("Erreur, réiterez votre demande");
            ht = Console.ReadLine();
            success = int.TryParse(ht, out hauteur);
          }
          Console.WriteLine("Veuillez saisir la largeur de votre rectangle");
          ht = Console.ReadLine();
          success = int.TryParse(ht, out largeur);
          while (largeur <= 0 || success != true)
          {
            Console.WriteLine("Erreur, réiterez votre demande");
            ht = Console.ReadLine();
            success = int.TryParse(ht, out largeur);
          }

          tab = CreerRectangle(hauteur, largeur);
        }
        else
        {
          //--- Motif Triangle ---
          if (motifName == "triangle")
          {
            Console.WriteLine("Veuillez saisir la hauteur de votre triangle");
            ht = Console.ReadLine();
            success = int.TryParse(ht, out hauteur);
            while (hauteur<=0 || success != true)
            {
              Console.WriteLine("Erreur, réiterez votre demande");
              ht = Console.ReadLine();
              success = int.TryParse(ht, out hauteur);
            }
            largeur = 2 * hauteur - 1;

            tab = CreerTriangle(hauteur);
          }
        }
        //--- Pour les limites ---
        hauteur += 2;
        largeur += 2;

        //--- Choix de la première position ---
        AfficherGrilleJoueur(tab, motifName, hauteur, largeur);
        Console.WriteLine("\nVeuillez choisir votre position de départ :");
            //--- Ligne ---
        Console.WriteLine("Veuillez saisir la ligne de départ");
        ht = Console.ReadLine();
        success = int.TryParse(ht, out findx);
        while ((findx > hauteur - 2) || (findx < 1) || (success != true))
        {
          Console.WriteLine("Erreur, réiterez votre demande");
          ht = Console.ReadLine();
          success = int.TryParse(ht, out findx);
        }
            //--- Colonne ---
        Console.WriteLine("Veuillez saisir la colonne de départ");
        ht = Console.ReadLine();
        success = int.TryParse(ht, out findy);
        while ((findy > largeur - 2) || (findy < 1) || (success != true))
        {
          Console.WriteLine("Erreur, réiterez votre demande");
          ht = Console.ReadLine();
          success = int.TryParse(ht, out findy);
        }

        //--
        EcrireAnciennePositionDeJeu(findx, findy, nb);//On a la position de départ.
        //--

        tab[findx][findy] = " J ";

        //--- Calcul du nombre de cases utiles ---
        nbCases = 0;
        if (motifName == "triangle")
        {
          for (int i = 0; i < hauteur - 2; i++)
          {
            nbCases += 2 * i + 1;
          }
        }
        else
        {
          if (motifName == "rectangle")
          {
            nbCases = hauteur * largeur;
          }
        }
        //--- Positionnement Mines + Trésor et récupération du nombre ---
        nbMines = PositionnerMines(tab, hauteur, largeur, nbCases, rng, findx, findy);  //On initialise les mines ainsi que les trésors en faisant attention que cela n'arrive pas sur la position de départ. 
        nbTresor = PositionnerTrésor(tab, hauteur, largeur, nbCases, rng, findx, findy); // De même pour les trésors

      }
      else 
      {
        //--- Charger une partie ---
        string[] positionJoueur = LireAnciennePositionDeJeu(nb);
        findx = int.Parse(positionJoueur[0]);
        findy = int.Parse(positionJoueur[1]);
        nbMines = 0;
        tab = grille;
        //--Calcul du nombre de mines--
        for (int i = 0; i < grille.Length; i++)
        {
          for (int j = 0; j < grille[1].Length; j++)
          {
            if (grille[i][j] == " M ")
            {
              nbMines++;
            }
          }
        }
      }
      //-- Dans tous les cas --

      //-- Calcul du nombre de cases utiles (refait si Recommencer, mais besoin pour placer les bombes. Pas de pb, C=O(1) --
      nbCases = 0;
      if (motifName == "triangle")
      {
        for (int i=0; i < hauteur - 2; i++)
        {
          nbCases += 2 * i + 1;
        }
      }
      else
      {
        if (motifName == "rectangle")
        {
          nbCases = (hauteur - 2) * (largeur - 2);
        }
      }
      int ligneJ = findx;
      int colonneJ = findy;

      //--
      EcrireUneSauvegarde(tab, score, etat, hauteur, largeur, saveTxt);
      etatsTab[nb] = "Plein";
      MettreAJourDesEtats(etatsTab);

      //-- Initialisation du jeu --
      bool stop = false;

      Reveler(tab, nbCases, hauteur, largeur, ligneJ, colonneJ, ref perdu); // On revèle lors de la première itération.
      string scoreCaseJoueur = tab[ligneJ][colonneJ];
      string pos = scoreCaseJoueur[0] + scoreCaseJoueur[1] + "-";
      tab[ligneJ][colonneJ] = " J ";
      tab[ligneJ][colonneJ] = pos;
      Console.WriteLine();

      //-- Affichage selon Admin Mode (ou non) --
      string AdminMode = ChoisirAdminMode();
      Console.Clear();
      if (AdminMode == "admin-oui")
      {
        AfficherGrille(tab, motifName, hauteur, largeur);
        Console.WriteLine();
      }

      AfficherGrilleJoueur(tab, motifName, hauteur, largeur);
      tab[ligneJ][colonneJ] = " J ";
      tab[ligneJ][colonneJ] = pos;

      //-- Boucle jusqu'à la fin du jeu --
      while (!stop)
      {
        //-- Gestion du score --
        score++;

        //-- Écriture sauvegarde --
        EcrireUneSauvegarde(tab, score, etat, hauteur, largeur, saveTxt);
        Console.WriteLine();
        tab[ligneJ][colonneJ] = scoreCaseJoueur;
        //-- Lecture de la nouvelle position --
            //-- Ligne --
        Console.WriteLine("Veuillez saisir la ligne de votre prochain coup");
        Console.ForegroundColor = ConsoleColor.Black;
        Console.BackgroundColor = ConsoleColor.White;
        ht = Console.ReadLine();
        success = int.TryParse(ht, out ligneJ);
        while ((ligneJ > hauteur - 2) || (ligneJ < 1) || (success != true))
        {
          Console.WriteLine("Erreur, réiterez votre demande");
          ht = Console.ReadLine();
          success = int.TryParse(ht, out ligneJ);
        }
        Console.ResetColor();
            //-- Colonne --
        Console.WriteLine("Veuillez saisir la colonne de votre prochaine coup");
        Console.ForegroundColor = ConsoleColor.Black;
        Console.BackgroundColor = ConsoleColor.White;
        ht = Console.ReadLine();
        success = int.TryParse(ht, out colonneJ);
        while ((colonneJ > largeur - 2) || (colonneJ < 1) || (success != true))
        {
          Console.WriteLine("Erreur, réiterez votre demande");
          ht = Console.ReadLine();
          success = int.TryParse(ht, out colonneJ);
        }
        Console.ResetColor();

        //-- Sauvegarde de la position de jeu --
        EcrireAnciennePositionDeJeu(ligneJ, colonneJ, nb);

        //-- Révélation --
        Reveler(tab, nbCases, hauteur, largeur, ligneJ, colonneJ, ref perdu);
        scoreCaseJoueur = tab[ligneJ][colonneJ];
        pos = scoreCaseJoueur[0] + scoreCaseJoueur[1] + "-";
        tab[ligneJ][colonneJ] = pos;

        //-- Affichage selon Admin Mode (ou non) --
        if (AdminMode == "admin-oui")
        {
          AfficherGrille(tab, "rectangle", hauteur, largeur);
          Console.WriteLine();
        }

        AfficherGrilleJoueur(tab, "rectangle", hauteur, largeur);
        AfficherScore(score);
        tab[ligneJ][colonneJ] = scoreCaseJoueur;

        //-- Test de fin de jeu --
        string x = FinirJeu(tab, perdu, nbCases, hauteur, largeur, nbMines);
        if (x == "Perdu")
        {
          stop = true; // Fin Boucle While
        }
        if (x == "Gagné")
        {
          stop = true; // Fin Boucle While
        }
      }
      //-- Fin du jeu --
          //-- Perdu --
      if (FinirJeu(tab, perdu, nbCases, hauteur, largeur, nbMines) == "Perdu")
      {
        AfficherPerdu();
        etatsTab[nb] = "Vide";
        EcrireMotifsSauvegarde(nb, "rectangle");
        etat = SupprimerUneSauvegarde(saveTxt, etatsTab);
      }
      else
      {
          //-- Gagné --
        etatsTab[nb] = "Vide";
        AfficherGagner(score);
        AfficherNouveauRecord(score);
        EcrireScore(score);
        EcrireMotifsSauvegarde(nb, "rectangle");
        etat = SupprimerUneSauvegarde(saveTxt, etatsTab);
      }
    }

    /*================================================================================================================================
     *                                                      Gestion des pointeurs.
     ================================================================================================================================*/
    //--- Choix de la save -----------------------------------------

        //-- Fonction de pointage --
    static public string ChoisirNumeroSave()
    {
      int etat = 1;
      AfficherNumeroSave1();
      var x = Console.ReadKey().Key;
      while (x != ConsoleKey.Enter)
      {
        if (x == ConsoleKey.RightArrow)
        {
          Console.Clear();
          if (etat == 4)
          {
            etat = 4;
            AfficherNumeroSave4();
          }
          if (etat == 3)
          {
            etat++;
            AfficherNumeroSave4();
          }
          if (etat == 2)
          {
            etat++;
            AfficherNumeroSave3();
          }
          if (etat == 1)
          {
            etat++;
            AfficherNumeroSave2();
          }
          x = Console.ReadKey().Key;
        }
        else
        {
          if (x == ConsoleKey.LeftArrow)
          {
            Console.Clear();
            if (etat == 2)
            {
              AfficherNumeroSave1();
              etat = 1;
            }
            if (etat == 3)
            {
              AfficherNumeroSave2();
              etat = 2;
            }
            if (etat == 4)
            {
              AfficherNumeroSave3();
              etat = 3;
            }
            if (etat == 1)
            {
              AfficherNumeroSave1();
            }
            x = Console.ReadKey().Key;
          }
          else
          {
            if (x != ConsoleKey.Enter)
            {
              if (etat == 1)
              {
                AfficherNumeroSave1();
              }
              if (etat == 2)
              {
                AfficherNumeroSave2();
              }
              if (etat == 3)
              {
                AfficherNumeroSave3();
              }
              if (etat == 4)
              {
                AfficherNumeroSave4();
              }
              x = Console.ReadKey().Key;
            }
          }
        }

      }
      if (etat == 1)
      {
        return "1";
      }
      else
      {
        if (etat == 2)
        {

          return "2";
        }
        else
        {
          if (etat == 3)
          {

            return "3";
          }
          else
          {
            return "4";
          }
        }
      }
    }
        //-- Différents affichages --
    static public void AfficherNumeroSave1()
    {
      CentrerTxt("Veuillez choisir l'un des 4 emplacements de sauvegarde (de 1 à 4).", 7);
      var x = 1 * Console.WindowWidth / 5 - " 1 ".Length / 2;
      Console.SetCursorPosition(x, 15);
      Console.BackgroundColor = ConsoleColor.White;
      Console.ForegroundColor = ConsoleColor.Black;
      Console.Write(" 1 ");
      Console.ResetColor();
      x = 2 * Console.WindowWidth / 5 - " 2 ".Length / 2;
      Console.SetCursorPosition(x, 15);
      Console.Write(" 2 ");
      x = 3 * Console.WindowWidth / 5 - " 3 ".Length / 2;
      Console.SetCursorPosition(x, 15);
      Console.Write(" 3 ");
      x = 4 * Console.WindowWidth / 5 - " 4 ".Length / 2;
      Console.SetCursorPosition(x, 15);
      Console.Write(" 4 ");
    }
    static public void AfficherNumeroSave2()
    {
      CentrerTxt("Veuillez choisir l'un des 4 emplacements de sauvegarde (de 1 à 4).", 7);
      var x = 1 * Console.WindowWidth / 5 - " 1 ".Length / 2;
      Console.SetCursorPosition(x, 15);
      Console.Write(" 1 ");
      Console.BackgroundColor = ConsoleColor.White;
      Console.ForegroundColor = ConsoleColor.Black;
      x = 2 * Console.WindowWidth / 5 - " 2 ".Length / 2;
      Console.SetCursorPosition(x, 15);
      Console.Write(" 2 ");
      Console.ResetColor();
      x = 3 * Console.WindowWidth / 5 - " 3 ".Length / 2;
      Console.SetCursorPosition(x, 15);
      Console.Write(" 3 ");
      x = 4 * Console.WindowWidth / 5 - " 4 ".Length / 2;
      Console.SetCursorPosition(x, 15);
      Console.Write(" 4 ");
    }
    static public void AfficherNumeroSave3()
    {
      CentrerTxt("Veuillez choisir l'un des 4 emplacements de sauvegarde (de 1 à 4).", 7);
      var x = 1 * Console.WindowWidth / 5 - " 1 ".Length / 2;
      Console.SetCursorPosition(x, 15);
      Console.Write(" 1 ");
      x = 2 * Console.WindowWidth / 5 - " 2 ".Length / 2;
      Console.SetCursorPosition(x, 15);
      Console.Write(" 2 ");
      Console.BackgroundColor = ConsoleColor.White;
      Console.ForegroundColor = ConsoleColor.Black;
      x = 3 * Console.WindowWidth / 5 - " 3 ".Length / 2;
      Console.SetCursorPosition(x, 15);
      Console.Write(" 3 ");
      Console.ResetColor();
      x = 4 * Console.WindowWidth / 5 - " 4 ".Length / 2;
      Console.SetCursorPosition(x, 15);
      Console.Write(" 4 ");
    }
    static public void AfficherNumeroSave4()
    {
      CentrerTxt("Veuillez choisir l'un des 4 emplacements de sauvegarde (de 1 à 4).", 7);
      var x = 1 * Console.WindowWidth / 5 - " 1 ".Length / 2;
      Console.SetCursorPosition(x, 15);
      Console.Write(" 1 ");
      x = 2 * Console.WindowWidth / 5 - " 2 ".Length / 2;
      Console.SetCursorPosition(x, 15);
      Console.Write(" 2 ");
      x = 3 * Console.WindowWidth / 5 - " 3 ".Length / 2;
      Console.SetCursorPosition(x, 15);
      Console.Write(" 3 ");
      Console.BackgroundColor = ConsoleColor.White;
      Console.ForegroundColor = ConsoleColor.Black;
      x = 4 * Console.WindowWidth / 5 - " 4 ".Length / 2;
      Console.SetCursorPosition(x, 15);
      Console.Write(" 4 ");
      Console.ResetColor();
    }

    //--- Choix "Recommencer" ou "Charger" une save ----------------

        //-- Fonction de pointage --
    static public string ChoisirRecommencerOuCharger()
    {
      Console.Clear();
      string etat = "Recommencer";
      AfficherRecommencer();
      var x = Console.ReadKey().Key;
      while (x != ConsoleKey.Enter)
      {
        if (x == ConsoleKey.LeftArrow)
        {
          Console.Clear();
          if (etat == "Recommencer")
          {
            AfficherRecommencer();
          }
          else
          {
            AfficherRecommencer();
            etat = "Recommencer";
          }
          x = Console.ReadKey().Key;
        }
        if (x == ConsoleKey.RightArrow)
        {
          Console.Clear();
          if (etat == "Recommencer")
          {
            etat = "Charger";
            AfficherCharger();
          }
          else
          {
            AfficherCharger();
            etat = "Charger";
          }
          x = Console.ReadKey().Key;
        }
        else
        {
          if (x != ConsoleKey.Enter)
          {
            if (etat == "Recommencer")
            {
              Console.Clear();
              AfficherRecommencer();
            }
            if (etat == "Charger")
            {
              Console.Clear();
              AfficherCharger();
            }
            x = Console.ReadKey().Key;
          }
        }

      }
      return etat;

    }
        //-- Différents affichages --
    static public void AfficherRecommencer()
    {
      CentrerTxt("Desirez - vous en charger la sauvegarde séléctionnée ? Ou voulez - vous écraser cette dernière afin de recommencer une partie ?", 7);
      var x = Console.WindowWidth / 3 - "Recommencer".Length / 2;
      Console.SetCursorPosition(x, 15);
      Console.BackgroundColor = ConsoleColor.White;
      Console.ForegroundColor = ConsoleColor.Black;
      Console.Write("Recommencer");
      Console.ResetColor();
      x = 2 * Console.WindowWidth / 3 - "Charger".Length / 2;
      Console.SetCursorPosition(x, 15);
      Console.Write("Charger");
    }
    static public void AfficherCharger()
    {
      CentrerTxt("Desirez - vous en charger la sauvegarde séléctionnée ? Ou voulez - vous écraser cette dernière afin de recommencer une partie ?", 7);
      var x = Console.WindowWidth / 3 - "Recommencer".Length / 2;
      Console.SetCursorPosition(x, 15);
      Console.Write("Recommencer");
      Console.BackgroundColor = ConsoleColor.White;
      Console.ForegroundColor = ConsoleColor.Black;
      x = 2 * Console.WindowWidth / 3 - "Charger".Length / 2;
      Console.SetCursorPosition(x, 15);
      Console.Write("Charger");
      Console.ResetColor();
    }

    //--- Choix du motif (triangle ou rectangle) -------------------

        //-- Fonction de pointage --
    static public string ChoisirTriangleOuRectangle()
    {
      Console.Clear();
      string etat = "rectangle";
      AfficherRectangle();
      var x = Console.ReadKey().Key;
      while (x != ConsoleKey.Enter)
      {
        if (x == ConsoleKey.LeftArrow)
        {
          Console.Clear();
          if (etat == "triangle")
          {
            etat = "rectangle";
            AfficherRectangle();
          }
          else
          {
            AfficherRectangle();
            etat = "rectangle";
          }
          x = Console.ReadKey().Key;
        }
        if (x == ConsoleKey.RightArrow)
        {
          Console.Clear();
          if (etat == "rectangle")
          {
            etat = "triangle";
            AfficherTriangle();
          }
          else
          {
            AfficherTriangle();
          }
          x = Console.ReadKey().Key;
        }
        else
        {
          if (x != ConsoleKey.Enter)
          {
            if (etat == "rectangle")
            {
              Console.Clear();
              AfficherRectangle();
            }
            if (etat == "triangle")
            {
              Console.Clear();
              AfficherTriangle();
            }
            x = Console.ReadKey().Key;
          }
        }

      }
      return etat;

    }
        //-- Différents affichages --
    static public void AfficherRectangle()
    {
      CentrerTxt("Veuillez saisir le motif désiré !", 7);
      var x = Console.WindowWidth / 3 - "Rectangle".Length / 2;
      Console.SetCursorPosition(x, 15);
      Console.BackgroundColor = ConsoleColor.White;
      Console.ForegroundColor = ConsoleColor.Black;
      Console.Write("Rectangle");
      Console.ResetColor();
      x = 2 * Console.WindowWidth / 3 - "Triangle".Length / 2;
      Console.SetCursorPosition(x, 15);
      Console.Write("Triangle");
    }
    static public void AfficherTriangle()
    {
      CentrerTxt("Veuillez saisir le motif désiré !", 7);
      var x = Console.WindowWidth / 3 - "Rectangle".Length / 2;
      Console.SetCursorPosition(x, 15);
      Console.Write("Rectangle");
      Console.BackgroundColor = ConsoleColor.White;
      Console.ForegroundColor = ConsoleColor.Black;
      x = 2 * Console.WindowWidth / 3 - "Triangle".Length / 2;
      Console.SetCursorPosition(x, 15);
      Console.Write("Triangle");
      Console.ResetColor();
    }

    //--- Choix du mode ADMIN --------------------------------------

        //-- Fonction de pointage --
    static public string ChoisirAdminMode()
    {
      Console.Clear();
      string etat = "admin-oui";
      AfficherAdminOui();
      var x = Console.ReadKey().Key;
      while (x != ConsoleKey.Enter)
      {
        if (x == ConsoleKey.DownArrow)
        {
          Console.Clear();
          AfficherAdminNon();
          etat = "admin-non";
          x = Console.ReadKey().Key;
        }
        if (x == ConsoleKey.UpArrow)
        {
          Console.Clear();
          AfficherAdminOui();
          etat = "admin-oui";
          x = Console.ReadKey().Key;
        }
        else
        {
          if (x != ConsoleKey.Enter)
          {
            if (etat == "admin-non")
            {
              AfficherAdminNon();
            }
            else
            {
              AfficherAdminOui();
            }
            x = Console.ReadKey().Key;
          }
        }
      }
      if (etat == "admin-non")
      {
        return "admin-non";
      }
      else
      {
        return "admin-oui";
      }
    }
        //-- Différents affichages --
    static public void AfficherAdminOui()
    {
      CentrerTxt("Voulez-vous activer le mode ADMIN (vision sur une grille en parallèle des bombes et des trésors) ?", 7);
      var x = Console.WindowWidth / 2 - "Oui".Length / 2;
      Console.SetCursorPosition(x, 15);
      Console.BackgroundColor = ConsoleColor.White;
      Console.ForegroundColor = ConsoleColor.Black;
      Console.Write("Oui");
      Console.ResetColor();
      x = Console.WindowWidth / 2 - "Oui".Length / 2;
      Console.SetCursorPosition(x, 20);
      Console.Write("Non");
    }
    static public void AfficherAdminNon()
    {
      CentrerTxt("Voulez-vous activer le mode ADMIN (vision sur une grille en parallèle des bombes et des trésors) ?", 7);
      var x = Console.WindowWidth / 2 - "Oui".Length / 2;
      Console.SetCursorPosition(x, 15);
      Console.Write("Oui");
      Console.BackgroundColor = ConsoleColor.White;
      Console.ForegroundColor = ConsoleColor.Black;
      x = Console.WindowWidth / 2 - "Non".Length / 2;
      Console.SetCursorPosition(x, 20);
      Console.Write("Non");
      Console.ResetColor();

    }

    //--- Gestion des menus d'accueil ------------------------------

        //-- Fonction de pointage --
    static public string AfficherEcranAccueil()
    {
      string etat = "jeu";
      AfficherMenuJouer();
      var x = Console.ReadKey().Key;
      while (x != ConsoleKey.Enter)
      {
        if (x == ConsoleKey.DownArrow)
        {
          Console.Clear();
          AfficherMenuStatistique();
          etat = "stat";
          x = Console.ReadKey().Key;
        }
        if (x == ConsoleKey.UpArrow)
        {
          Console.Clear();
          AfficherMenuJouer();
          etat = "jeu";
          x = Console.ReadKey().Key;
        }
        else
        {
          if (x != ConsoleKey.Enter)
          {
            if (etat == "jeu")
            {
              AfficherMenuJouer();
              x = Console.ReadKey().Key;
            }
            else
            {
              AfficherMenuStatistique();
              x = Console.ReadKey().Key;
            }
          }
        }
      }
      return etat;
    } //d
        //-- Différents affichages --
    static public void AfficherMenuJouer()
    {

      var name = new string[]
      {

@" ,-----.,--.                                                            ,--.                                         ",
@"'  .--./|  ,---.  ,--,--. ,---.  ,---.  ,---.      ,--,--.,--.,--.    ,-'  '-.,--.--. ,---.  ,---.  ,---. ,--.--.    ",
@"|  |    |  .-.  |' ,-.  |(  .-' (  .-' | .-. :    ' ,-.  ||  ||  |    '-.  .-'|  .--'| .-. :(  .-' | .-. ||  .--'    ",
@"'  '--'\|  | |  |\ '-'  |.-'  `).-'  `)\   --.    \ '-'  |'  ''  '      |  |  |  |   \   --..-'  `)' '-' '|  |",
@" `-----'`--' `--' `--`--'`----' `----'  `----'     `--`--' `----'       `--'  `--'    `----'`----'  `---' `--'       ",


      };
      var maxLength = name.Aggregate(0, (max, line) => Math.Max(max, line.Length));
      var xt = Console.BufferWidth / 2 - maxLength / 2;
      DessinerConsole(name, 2, 2);
      var x = Console.WindowWidth / 2 - "Jouer".Length / 2;
      Console.SetCursorPosition(x, 15);
      Console.BackgroundColor = ConsoleColor.White;
      Console.ForegroundColor = ConsoleColor.Black;
      Console.Write("Jouer");
      Console.ResetColor();
      x = Console.WindowWidth / 2 - "Statistique".Length / 2;
      Console.SetCursorPosition(x, 20);
      Console.Write("Statistiques");
    }
    static public void AfficherMenuStatistique()
    {
      var name = new string[]
      {

@" ,-----.,--.                                                            ,--.                                         ",
@"'  .--./|  ,---.  ,--,--. ,---.  ,---.  ,---.      ,--,--.,--.,--.    ,-'  '-.,--.--. ,---.  ,---.  ,---. ,--.--.    ",
@"|  |    |  .-.  |' ,-.  |(  .-' (  .-' | .-. :    ' ,-.  ||  ||  |    '-.  .-'|  .--'| .-. :(  .-' | .-. ||  .--'    ",
@"'  '--'\|  | |  |\ '-'  |.-'  `).-'  `)\   --.    \ '-'  |'  ''  '      |  |  |  |   \   --..-'  `)' '-' '|  |",
@" `-----'`--' `--' `--`--'`----' `----'  `----'     `--`--' `----'       `--'  `--'    `----'`----'  `---' `--'       ",


      };
      var maxLength = name.Aggregate(0, (max, line) => Math.Max(max, line.Length));
      var xt = Console.BufferWidth / 2 - maxLength / 2;
      DessinerConsole(name, 2, 2);
      var x = Console.WindowWidth / 2 - "Jouer".Length / 2;
      Console.SetCursorPosition(x, 15);
      Console.Write("Jouer");
      Console.SetCursorPosition(x, 25);
      Console.BackgroundColor = ConsoleColor.White;
      Console.ForegroundColor = ConsoleColor.Black;
      x = Console.WindowWidth / 2 - "Statistique".Length / 2;
      Console.SetCursorPosition(x, 20);
      Console.Write("Statistiques");
      Console.ResetColor();
    }

    //--- Choix refaire une partie (ou non) ------------------------
        //-- Fonction de pointage --
    static public bool ChoisirRefairePartie()
    {
      Console.Clear();
      string etat = "oui";
      AfficherOui();
      var x = Console.ReadKey().Key;
      while (x != ConsoleKey.Enter)
      {
        if (x == ConsoleKey.LeftArrow)
        {
          Console.Clear();
          if (etat == "non")
          {
            etat = "oui";
            AfficherOui();
          }
          else
          {
            AfficherOui();
            etat = "oui";
          }
          x = Console.ReadKey().Key;
        }
        if (x == ConsoleKey.RightArrow)
        {
          Console.Clear();
          if (etat == "oui")
          {
            etat = "non";
            AfficherNon();
          }
          else
          {
            AfficherNon();
          }
          x = Console.ReadKey().Key;
        }
        else
        {
          if (x != ConsoleKey.Enter)
          {
            if (etat == "oui")
            {
              Console.Clear();
              AfficherOui();
            }
            if (etat == "non")
            {
              Console.Clear();
              AfficherNon();
            }
            x = Console.ReadKey().Key;
          }
        }

      }
      if (etat == "oui")
      {
        return true;
      }
      else
      {
        return false;
      }

    }
        //-- Différents affichages --
    static public void AfficherOui()
    {
      CentrerTxt("Voulez-vous refaire une partie ?", 7);
      var x = Console.WindowWidth / 3 - "Oui".Length / 2;
      Console.SetCursorPosition(x, 15);
      Console.BackgroundColor = ConsoleColor.White;
      Console.ForegroundColor = ConsoleColor.Black;
      Console.Write("Oui");
      Console.ResetColor();
      x = 2 * Console.WindowWidth / 3 - "Non".Length / 2;
      Console.SetCursorPosition(x, 15);
      Console.Write("Non");
    }
    static public void AfficherNon()
    {
      CentrerTxt("Voulez-vous refaire une partie ?", 7);
      var x = Console.WindowWidth / 3 - "Oui".Length / 2;
      Console.SetCursorPosition(x, 15);
      Console.Write("Oui");
      Console.BackgroundColor = ConsoleColor.White;
      Console.ForegroundColor = ConsoleColor.Black;
      x = 2 * Console.WindowWidth / 3 - "Non".Length / 2;
      Console.SetCursorPosition(x, 15);
      Console.Write("Non");
      Console.ResetColor();
    }

    //--- Affichage menu statistiques ------------------------------
    static public void AfficherStatistiquesDeJeu()
    {
      Console.Clear();

      if (!System.IO.File.Exists("ScoreSaver.txt")) //On a pas encore gagné une seule fois
      {
        if (!System.IO.File.Exists("ScoreSaver.txt")) //Pas de nom d'utilisateur renseigné.
        {
          Console.ResetColor();
          var name = new string[]
          {


@" ,---.   ,--.            ,--.  ,--.        ,--.  ,--.    ",
@"'   .-',-'  '-. ,--,--.,-'  '-.`--' ,---.,-'  '-.`--' ,---. ,--.,--. ,---.  ,---.     ",
@"`.  `-.'-.  .-'' ,-.  |'-.  .-',--.(  .-''-.  .-',--.| .-. ||  ||  || .-. :(  .-'     ",
@".-'    | |  |  \ '-'  |  |  |  |  |.-'  `) |  |  |  |' '-' |'  ''  '\   --..-'  `)    ",
@"`-----'  `--'   `--`--'  `--'  `--'`----'  `--'  `--' `-|  | `----'  `----'`----'     ",
@"                                                        `--'                          "



          };
          var maxLength = name.Aggregate(0, (max, line) => Math.Max(max, line.Length));
          var xt = Console.BufferWidth / 2 - maxLength / 2;
          DessinerConsole(name, xt, 2);
          Console.WriteLine();
          Console.WriteLine("Aucune partie n'a encore été gagnée, la page de statistique n'est donc pas disponible.De plus, aucun nom d'utilisateur n'a été renseigné. \nVeuillez nous excuser pour ce contre-temps.");
        }
        else
        {
          string[] lines = File.ReadAllLines(@"NomUser.txt");
          string username = lines[0];
          Console.BackgroundColor = ConsoleColor.DarkBlue;
          Console.ForegroundColor = ConsoleColor.Black;
          CentrerTxt(" Bienvenue " + username + " ", 7);
          Console.ResetColor();
          var name = new string[]
          {


@" ,---.   ,--.            ,--.  ,--.        ,--.  ,--.    ",
@"'   .-',-'  '-. ,--,--.,-'  '-.`--' ,---.,-'  '-.`--' ,---. ,--.,--. ,---.  ,---.     ",
@"`.  `-.'-.  .-'' ,-.  |'-.  .-',--.(  .-''-.  .-',--.| .-. ||  ||  || .-. :(  .-'     ",
@".-'    | |  |  \ '-'  |  |  |  |  |.-'  `) |  |  |  |' '-' |'  ''  '\   --..-'  `)    ",
@"`-----'  `--'   `--`--'  `--'  `--'`----'  `--'  `--' `-|  | `----'  `----'`----'     ",
@"                                                        `--'                          "



          };
          var maxLength = name.Aggregate(0, (max, line) => Math.Max(max, line.Length));
          var xt = Console.BufferWidth / 2 - maxLength / 2;
          DessinerConsole(name, xt, 2);
          Console.WriteLine();
          Console.WriteLine("Aucune partie n'a encore été gagnée, la page de statistique n'est donc pas disponible.\nVeuillez nous excuser pour ce contre-temps.");
        }
      }
      else
      {
        string[] scoresTableau = File.ReadAllLines("ScoreSaver.txt");


        /* Récupère le code utile sous la forme (string):
                                  *           H_hauteur_largeur_score
                                  * Avec :
                                  *   - H = V (Vide) / P (Plein)  

                                  * 
                                  * De plus :
                                  *   - hauteur = hauteur de la grille
                                  *   - largeur = largeur de la grille
                                  *   - score = score actuel
                                  */
        string[] lines = File.ReadAllLines(@"NomUser.txt");
        string username = lines[0];
        var name = new string[]
        {


@" ,---.   ,--.            ,--.  ,--.        ,--.  ,--.    ",
@"'   .-',-'  '-. ,--,--.,-'  '-.`--' ,---.,-'  '-.`--' ,---. ,--.,--. ,---.  ,---.     ",
@"`.  `-.'-.  .-'' ,-.  |'-.  .-',--.(  .-''-.  .-',--.| .-. ||  ||  || .-. :(  .-'     ",
@".-'    | |  |  \ '-'  |  |  |  |  |.-'  `) |  |  |  |' '-' |'  ''  '\   --..-'  `)    ",
@"`-----'  `--'   `--`--'  `--'  `--'`----'  `--'  `--' `-|  | `----'  `----'`----'     ",
@"                                                        `--'                          "



        };
        var maxLength = name.Aggregate(0, (max, line) => Math.Max(max, line.Length));
        var xt = Console.BufferWidth / 2 - maxLength / 2;
        DessinerConsole(name, xt, 2);

        Console.BackgroundColor = ConsoleColor.DarkBlue;
        Console.ForegroundColor = ConsoleColor.White;
        CentrerTxt(" Bienvenue " + username + " ", 7);
        Console.BackgroundColor = ConsoleColor.DarkRed;
        CentrerTxt("Appuyez sur Echap pour revenir au menu", 8);
        Console.ResetColor();

        int sum = 0;
        int nb = scoresTableau.Length;
        for (int i = 0; i < scoresTableau.Length; i++)
        {
          sum += int.Parse(scoresTableau[i]);
        }
        Console.BackgroundColor = ConsoleColor.White;
        Console.ForegroundColor = ConsoleColor.Black;
        CentrerTxt(" Moyenne des scores ", 10);
        Console.ResetColor();
        CentrerTxt((sum / nb).ToString(), 11);
        Console.WriteLine();

        Console.BackgroundColor = ConsoleColor.White;
        Console.ForegroundColor = ConsoleColor.Black;
        CentrerTxt(" Meilleur score ", 13);
        Console.ResetColor();
        CentrerTxt(scoresTableau.Min(), 14);
        Console.WriteLine();


        CentrerTxt("===================================", 16);
        Console.BackgroundColor = ConsoleColor.White;
        Console.ForegroundColor = ConsoleColor.Black;
        CentrerTxt(" Répartition des scores ", 17);
        Console.ResetColor();
        CentrerTxt("===================================", 18);

        Console.WriteLine();
        Console.WriteLine();
        Console.WriteLine();
        Console.WriteLine();



        int[] interv = new int[10];
        int f = 0;
        int maxi = 100;
        int sep = maxi / 10;
        int inter = 10;
        int haut = sep;
        int count = 0;
        int bas = 0;
        while (haut != (inter) * sep)
        {
          for (int k = 0; k < nb; k++)
          {
            if (int.Parse(scoresTableau[k]) <= haut - 1)
            {
              if (int.Parse(scoresTableau[k]) >= bas)
              {
                count++;
              }
            }
          }

          interv[f] = count;
          count = 0;
          f++;
          bas += sep;
          haut += sep;

        }

        for (int k = 0; k < nb; k++)
        {
          if (int.Parse(scoresTableau[k]) <= maxi)
          {
            if (int.Parse(scoresTableau[k]) >= (inter - 1) * sep)
            {
              count++;
            }
          }
        }
        interv[f] = count;




        int nbLignes = 1;
        for (int i = 0; i < inter - 1; i++)
        {
          if (interv[i] > nbLignes)
          {
            nbLignes = interv[i];
          }
        }
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Red;
        for (int j = nbLignes; j >= 1; j--)
        {
          for (int p = 0; p <= inter - 1; p++)
          {
            if (j == interv[p])
            {
              Console.Write("######   ");
              interv[p]--;
            }
            else
            {
              Console.Write("         ");
            }
          }
          Console.WriteLine();
        }
        Console.ResetColor();


        Console.WriteLine("--------------------------------------------------------------------------------------------------");

        int lio;
        int lip;

        lio = 0;
        lip = sep - 1;
        while (lio != (inter - 1) * sep)
        {
          Console.Write(lio);
          Console.Write(" - ");
          Console.Write(lip);
          Console.Write(" I ");
          lio += sep;
          lip += sep;

        }
        Console.Write(lio);
        Console.Write(" - ");
        Console.Write(100);
        Console.Write(" I ");

      }
    }

    /*================================================================================================================================
     *                                           Fonctions esthétiques
     ================================================================================================================================*/
    //-- Affichage règle -- o
    static public void AfficherRegle()
    {

      Console.WindowWidth = 160;
      Console.WriteLine("\n\n");



      var bjr = new[]
      {
 @"  ___  ___  _  _    _  ___  _   _ ___   ___ _____   ___ ___ ___ _  ___   _____ _  _ _   _ ___    _ ",
 @" | _ )/ _ \| \| |_ | |/ _ \| | | | _ \ | __|_   _| | _ )_ _| __| \| \ \ / / __| \| | | | | __|  | |",
 @" | _ \ (_) | .` | || | (_) | |_| |   / | _|  | |   | _ \| || _|| .` |\ V /| _|| .` | |_| | _|   |_|",
 @" |___/\___/|_|\_|\__/ \___/ \___/|_|_\ |___| |_|   |___/___|___|_|\_| \_/ |___|_|\_|\___/|___|  (_)",
      };

      var maxLength = bjr.Aggregate(0, (max, line) => Math.Max(max, line.Length));
      var x = Console.BufferWidth / 2 - maxLength / 2;

      DessinerConsole(bjr, x, 2);
      Console.WriteLine("                                                                                       ");
      Console.WriteLine("                                                                                                                                              ");
      Console.WriteLine("                                                         Voici le déroulé du jeu                                                                                        ");
      Console.WriteLine("-Tout d'abord, nous vous demandrons de choisir un motif, parmis les motifs suivants :                                                          ");
      Console.WriteLine("-Nous vous demandrons ensuite de choisir les paramètres concernant ce motif (un rayon, un côté ...)                                            ");
      Console.WriteLine("                                                                                                                                               ");
      Console.WriteLine("-A chaque tour, vous allez devoir choisir une case (en nous indiquant une ligne et une colonne)                                                ");
      Console.BackgroundColor = ConsoleColor.DarkRed;
      Console.ForegroundColor = ConsoleColor.White;
      Console.WriteLine();
      Console.WriteLine("Attention : ");
      Console.WriteLine();
      Console.ResetColor();
      Console.WriteLine("Votre score augmente de 1 à chaque tour !                                                                                                      ");
      Console.WriteLine("Le but du jeu est de découvrir toutes les cases non minées ainsi que les trésors :) !                                                          ");
      Console.WriteLine("                                                                                                                                                ");
      Console.WriteLine("Le jeu s'arrête lorsque vous tombez sur une mine ! Ou lorsque vous gagnez bien évidemment !                                                    ");
      Console.WriteLine("Si vous tombez sur une case vide, vous revelez celles adjacentes selon un principe récursif !                                                   ");
      Console.WriteLine("                                                                                                                                                ");
      Console.WriteLine("                                                                                                                                               ");
      Console.WriteLine("Un nombre apparaît sur certaines cases après un choix de case ! Il représente pour les cases adjacentes la présence d'un trésor ou d'une mine !");
      Console.WriteLine("Cela est calculé comme ceci :                                                                                                                   ");
      Console.BackgroundColor = ConsoleColor.Red;
      Console.ForegroundColor = ConsoleColor.Black;
      Console.WriteLine("- +1 si on a une mine !");
      Console.BackgroundColor = ConsoleColor.Yellow;
      Console.ForegroundColor = ConsoleColor.Black;
      Console.WriteLine("- +2 si on a un trésor !");
      Console.ResetColor();
      Console.WriteLine("Concernant les couleurs de la grille !");

      AfficherVide();

      Console.Write("représente une case non visitée.");
      Console.WriteLine();
      AfficherJoueur("  ");

      Console.Write("représente votre dernier coup.");
      Console.WriteLine();

      AfficherVisitéTresor();

      Console.WriteLine("représente un trésor visité");

      AfficherNumero(" 0 ");

      Console.WriteLine("représente un décompte !");

      AfficherVisité();

      Console.WriteLine("représente une case déjà visitée.");
      Console.ForegroundColor = ConsoleColor.White;
      Console.BackgroundColor = ConsoleColor.Black;

    }

    //-- Centrer texte -- o
    static public void CentrerTxt(string mot, int ligne)
    {
      var x = Console.WindowWidth / 2 - mot.Length / 2;
      Console.SetCursorPosition(x, ligne);
      Console.Write(mot);
    }

    //-- Afficher l'introduction --  o
    static public void AfficherIntro()
    {
      var arr = new[]
      {
@" ________  ________  ________         ___  ________  ___  ___  ________          _______  _________               ",
@" |\   __  \|\   __  \|\   ___  \      |\  \|\   __  \|\  \|\  \|\   __  \        |\  ___ \|\___   ___\           ",
@" \ \  \|\ /\ \  \|\  \ \  \\ \  \     \ \  \ \  \|\  \ \  \\\  \ \  \|\  \       \ \   __/\|___ \  \_|",
@"  \ \   __  \ \  \\\  \ \  \\ \  \  __ \ \  \ \  \\\  \ \  \\\  \ \   _  _\       \ \  \_|/__  \ \  \   ",
@"   \ \  \|\  \ \  \\\  \ \  \\ \  \|\  \\_\  \ \  \\\  \ \  \\\  \ \  \\  \|       \ \  \_|\ \  \ \  \    ",
@"   \ \_______\ \_______\ \__\\ \__\ \________\ \_______\ \_______\ \__\\ _\        \ \_______\  \ \__\      ",
@"    \|_______|\|_______|\|__| \|__|\|________|\|_______|\|_______|\|__|\|__|        \|_______|   \|__|",
@"",
@" ",
@"",
@"   _______  ___  _______   ________   ___      ___ _______   ________   ___  ___  _______              ___",
@" |\   __  \|\  \|\  ___ \ |\   ___  \|\  \    /  /|\  ___ \ |\   ___  \|\  \|\  \|\  ___ \            |\  \      ",
@" \ \  \|\ /\ \  \ \   __/|\ \  \\ \  \ \  \  /  / | \   __/|\ \  \\ \  \ \  \\\  \ \   __/|           \ \  \     ",
@"  \ \   __  \ \  \ \  \_|/_\ \  \\ \  \ \  \/  / / \ \  \_|/_\ \  \\ \  \ \  \\\  \ \  \_|/__          \ \  \    ",
@"   \ \  \|\  \ \  \ \  \_|\ \ \  \\ \  \ \    / /   \ \  \_|\ \ \  \\ \  \ \  \\\  \ \  \_|\ \          \ \__\   ",
@"    \ \_______\ \__\ \_______\ \__\\ \__\ \__/ /     \ \_______\ \__\\ \__\ \_______\ \_______\          \|__|    ",
@"     \|_______|\|__|\|_______|\|__| \|__|\|__|/       \|_______|\|__| \|__|\|_______|\|_______|              ___  ",
@"                                                                                                            |\__\ ",
@"                                                                                                            \|__| ",

};
      Console.WindowWidth = 160;
      Console.WriteLine("\n\n");
      var maxLength = arr.Aggregate(0, (max, line) => Math.Max(max, line.Length));
      var x = Console.BufferWidth / 2 - maxLength / 2;
      for (int y = -arr.Length; y < Console.WindowHeight + arr.Length; y++)
      {
        DessinerConsole(arr, x, y);
        System.Threading.Thread.Sleep(20);
      }

      Console.Clear();
    }

    //-- Afficher ASCII-ART --  o
    static public void DessinerConsole(IEnumerable<string> lines, int x, int y)
    {
      if (x > Console.WindowWidth) return;
      if (y > Console.WindowHeight) return;

      var trimLeft = x < 0 ? -x : 0;
      int index = y;

      x = x < 0 ? 0 : x;
      y = y < 0 ? 0 : y;

      var linesToPrint =
          from line in lines
          let currentIndex = index++
          where currentIndex > 0 && currentIndex < Console.WindowHeight
          select new
          {
            Text = new String(line.Skip(trimLeft).Take(Math.Min(Console.WindowWidth - x, line.Length - trimLeft)).ToArray()),
            X = x,
            Y = y++
          };

      Console.Clear();
      foreach (var line in linesToPrint)
      {
        Console.SetCursorPosition(line.X, line.Y);
        Console.Write(line.Text);
      }
    }

    //--- Affichage des différents types de cases --- o
    static public void AfficherVide()
    {
      Console.BackgroundColor = ConsoleColor.White;
      Console.ForegroundColor = ConsoleColor.White;
      Console.Write("___");
      Console.ResetColor();
      Console.Write("   ");
    } //Affiche les cases non visitées / Vide
    static public void AfficherMines()
    {
      Console.BackgroundColor = ConsoleColor.Red;
      Console.ForegroundColor = ConsoleColor.White;
      Console.Write(" M "); 
      Console.ResetColor();
      Console.Write("   ");
    } //Affiche les mines sur la vue admin
    static public void AfficherTrésor()
    {
      Console.BackgroundColor = ConsoleColor.Yellow;
      Console.ForegroundColor = ConsoleColor.Black;
      Console.Write(" T ");
      Console.ResetColor();
      Console.Write("   ");
    } //Affiche les trésors sur la vue admin
    static public void AfficherJoueur(string x)
    {
      Console.BackgroundColor = ConsoleColor.Green;
      Console.ForegroundColor = ConsoleColor.White;
      char n = x[1];
      Console.Write(" " + n + " ");
      Console.ResetColor();
      Console.Write("   ");
    } //Affiche la position du joueur
    static public void AfficherVisité()
    {
      Console.BackgroundColor = ConsoleColor.DarkGray;
      Console.ForegroundColor = ConsoleColor.DarkGray;
      Console.Write(" _ ");
      Console.ResetColor();
      Console.Write("   ");
    } //Affiche une case visitée
    static public void AfficherNonUtile()
    {
      Console.BackgroundColor = ConsoleColor.Black;
      Console.ForegroundColor = ConsoleColor.Black;
      Console.Write(" _ ");
      Console.ResetColor();
      Console.Write("   ");
    }
    static public void AfficherVisitéTresor()
    {
      Console.BackgroundColor = ConsoleColor.DarkYellow;
      Console.ForegroundColor = ConsoleColor.DarkYellow;
      Console.Write(" _ ");
      Console.ResetColor();
      Console.Write("   ");
    }//Affiche une case trésor visitée
    static public void AfficherNumero(string x)
    {
      Console.BackgroundColor = ConsoleColor.DarkMagenta;
      Console.ForegroundColor = ConsoleColor.White;
      Console.Write(x);
      Console.ResetColor();
      Console.Write("   ");
    } //Affiche les décomptes

    //--- Affichage du score --- o
    static public void AfficherScore(int score)
    {
      string txt = "";
      txt += score;
      txt += @".txt";
      string text = File.ReadAllText(@txt);
      Console.WriteLine();
      Console.Write(text);
      Console.WriteLine();
    } //Rq : On a les nombres que jusqu'à 55.

    //--- Affichage si on fait un nouveau record ou non. o
    static public void AfficherNouveauRecord(int score)
    {
      if (!System.IO.File.Exists("ScoreSaver.txt")) //On a pas encore gagné une seule fois
      {
        Console.WriteLine("Nouveau record !");
      }

      else
      {
        string[] scoresTableau = File.ReadAllLines("ScoreSaver.txt");
        int scoreMaximum = int.Parse(scoresTableau[0]);
        for (int i = 0; i < scoresTableau.Length; i++)
        {
          if (int.Parse(scoresTableau[i]) < scoreMaximum)
          {
            scoreMaximum = int.Parse(scoresTableau[i]);
          }
        }
        if (score < scoreMaximum)
        {
          Console.WriteLine("Nouveau record !");
        }
      }


    }

    //--- Affichage de grilles --- o

    static public void AfficherGrille(string[][] tab, string motifName, int hauteur, int largeur)
    {
      int l = 1;
      int c = 1;
      if (motifName == "rectangle")
      {
        for (int i = 1; i < hauteur - 1; i++) //Pour un rectangle
        {
          if (l < 10)
          {
            Console.WriteLine();
            Console.WriteLine();
            Console.Write(" " + l + " ");
            Console.Write("  ");
            l++;
          }
          else
          {
            Console.WriteLine();
            Console.WriteLine();
            Console.Write(" " + l);
            Console.Write("  ");
            l++;
          }
          for (int j = 1; j < largeur - 1; j++)
          {
            if (tab[i][j] == "___")
            {
              AfficherVide();
            }
            else
            {
              if (tab[i][j] == "XXX")
              {
                AfficherNonUtile();
              }
              else
              {

                if (tab[i][j] == " M ")
                {
                  AfficherMines();
                }
                else
                {
                  if (tab[i][j] == " T ")
                  {
                    AfficherTrésor();
                  }
                  else
                  {

                    int n = tab[i][j].Length;
                    if (tab[i][j][n - 1] == '-')
                    {
                      AfficherJoueur(tab[i][j]);
                    }
                    else
                    {


                      if (tab[i][j] == " _ ")
                      {
                        AfficherVisité();
                      }
                      else
                      {
                        if (tab[i][j] == " V ")
                        {
                          AfficherVisitéTresor();
                        }
                        else
                        {
                          AfficherNumero(tab[i][j]);
                        }
                      }
                    }
                  }
                }
              }
            }
          }

        }
        Console.WriteLine();
        Console.WriteLine();
        Console.Write("     ");
        for (int i = 1; i < largeur - 1; i++)
        {
          if (c < 10)
          {
            Console.Write(" " + c + " ");
            Console.Write("   ");
            c++;
          }
          else
          {
            Console.Write(c + " ");
            Console.Write("   ");
            c++;
          }
        }
      }
      else
      {
        if (motifName == "triangle")
        {
          for (int i = 1; i < hauteur - 1; i++) //Pour un triangle
          {
            if (l < 10)
            {
              Console.WriteLine();
              Console.WriteLine();
              Console.Write(" " + l + " ");
              Console.Write("  ");
              l++;
            }
            else
            {
              Console.WriteLine();
              Console.WriteLine();
              Console.Write(" " + l);
              Console.Write("  ");
              l++;
            }
            for (int j = 1; j < largeur - 1; j++)
            {
              if (tab[i][j] == "___")
              {
                AfficherVide();
              }
              else
              {
                if (tab[i][j] == "XXX")
                {
                  AfficherNonUtile();
                }
                else
                {

                  if (tab[i][j] == " M ")
                  {
                    AfficherMines();
                  }
                  else
                  {
                    if (tab[i][j] == " T ")
                    {
                      AfficherTrésor();
                    }
                    else
                    {

                      int n = tab[i][j].Length;
                      if (tab[i][j][n - 1] == '-')
                      {
                        AfficherJoueur(tab[i][j]);
                      }
                      else
                      {


                        if (tab[i][j] == " _ ")
                        {
                          AfficherVisité();
                        }
                        else
                        {
                          if (tab[i][j] == " V ")
                          {
                            AfficherVisitéTresor();
                          }
                          else
                          {
                            AfficherNumero(tab[i][j]);
                          }
                        }
                      }
                    }
                  }
                }
              }
            }

          }
          Console.WriteLine();
          Console.WriteLine();
          Console.Write("     ");
          for (int i = 1; i < largeur - 1; i++)
          {
            if (c < 10)
            {
              Console.Write(" " + c + " ");
              Console.Write("   ");
              c++;
            }
            else
            {
              Console.Write(c + " ");
              Console.Write("   ");
              c++;
            }
          }
        }
      }
    } //Affiche la grille vue admin

    static public void AfficherGrilleJoueur(string[][] tab, string motifName, int hauteur, int largeur)
    {
      int l = 1;
      int c = 1;
      for (int i = 1; i < hauteur - 1; i++) //Pour un rectangle
      {
        if (l < 10)
        {
          Console.WriteLine();
          Console.WriteLine();
          Console.Write(" " + l + " ");
          Console.Write("  ");
          l++;
        }
        else
        {
          Console.WriteLine();
          Console.WriteLine();
          Console.Write(" " + l);
          Console.Write("  ");
          l++;
        }
        for (int j = 1; j < largeur - 1; j++)
        {
          if (tab[i][j] == "___")
          {
            AfficherVide();
          }
          else
          {
            if (tab[i][j] == "XXX")
            {
              AfficherNonUtile();
            }
            else
            {
              if (tab[i][j] == " M ")
              {
                AfficherVide();
              }
              else
              {
                if (tab[i][j] == " T ")
                {
                  AfficherVide();
                }
                else
                {

                  int n = tab[i][j].Length;
                  if (tab[i][j][n - 1] == '-')
                  {
                    AfficherJoueur(tab[i][j]);
                  }
                  else
                  {


                    if (tab[i][j] == " _ ")
                    {
                      AfficherVisité();
                    }
                    else
                    {
                      if (tab[i][j] == " V ")
                      {
                        AfficherVisitéTresor();
                      }
                      else
                      {
                        AfficherNumero(tab[i][j]);
                      }
                    }
                  }
                }
              }
            }
          }
        }

      }
      Console.WriteLine();
      Console.WriteLine();
      Console.Write("     ");
      for (int i = 1; i < largeur - 1; i++)
      {
        if (c < 10)
        {
          Console.Write(" " + c + " ");
          Console.Write("   ");
          c++;
        }
        else
        {
          Console.Write(c + " ");
          Console.Write("   ");
          c++;
        }
      }

    } //Affiche la grille vue joueur

    //--- Affichage Perdre / Gagner --- o

    static public void AfficherPerdu()
    {
      Exploser();
      Console.BackgroundColor = ConsoleColor.Red;
      Console.ForegroundColor = ConsoleColor.White;
      var perduTexte = new[] {

@"         ______          _____                                                    ",
@"   _____|\     \    _____\    \ ___________       ____________ ______   _____",
@"  /     / |     |  /    / |    |\          \      \           \\     \  \    \    ",
@" |      |/     /| /    /  /___/| \    /\    \      \           \\    |  |    | ",
@" |      |\____/ ||    |__ |___|/  |   \_\    |      |    /\     ||   |  |    |    ",
@" |\     \    | / |       \        |      ___/       |   |  |    ||    \_/   /| ",
@" | \     \___|/  |     __/ __     |      \  ____    |    \/     ||\         \|    ",
@" |  \     \      |\    \  /  \   /     /\ \/    \  /           /|| \         \__",
@"  \  \_____\     | \____\/    | /_____/ |\______| /___________/ | \ \_____/\    \ ",
@"   \ |     |     | |    |____/| |     | | |     ||           | /   \ |    |/___/| ",
@"    \|_____|      \|____|   | | |_____|/ \|_____||___________|/     \|____|   | | ",
@"                        |___|/                                            |___|/  "};

      Console.WriteLine("                                                                                                       ");
      DessinerConsole(perduTexte, 2, 2);
      Console.ResetColor();
      System.Threading.Thread.Sleep(3000);


    } //Animation si on perds

    static public void AfficherGagner(int score)
    {
      Exploser();
      Console.BackgroundColor = ConsoleColor.Green;
      Console.ForegroundColor = ConsoleColor.White;
      var gagnerTexte = new[] {

@"",
@"                                                                                   _____",
@"        _____           _____                _____       _____    _____       _____\    \ ___________",
@"   _____\    \_       /      |_         _____\    \_    |\    \   \    \     /    / |    |\          \      ",
@"  /     /|     |     /         \       /     /|     |    \\    \   |    |   /    /  /___/| \    /\    \     ",
@" /     / /____/|    |     /\    \     /     / /____/|     \\    \  |    |  |    |__ |___|/  |   \_\    |",
@"|     | |_____|/    |    |  |    \   |     | |_____|/      \|    \ |    |  |       \        |      ___/",
@"|     | |_________  |     \/      \  |     | |_________     |     \|    |  |     __/ __     |      \  ____",
@"|\     \|\        \ |\      /\     \ |\     \|\        \   /     /\      \ |\    \  /  \   /     /\ \/    \ ",
@"| \_____\|    |\__/|| \_____\ \_____\| \_____\|    |\__/| /_____/ /______/|| \____\/    | /_____/ |\______|",
@"| |     /____/| | ||| |     | |     || |     /____/| | |||      | |     | || |    |____/| |     | | |     |",
@" \|_____|     |\|_|/ \|_____|\|_____| \|_____|     |\|_|/|______|/|_____|/  \|____|   | | |_____|/ \|_____| ",
@"        |____/                               |____/                               |___|/ " };


      Console.WriteLine("                                                                                                       ");
      DessinerConsole(gagnerTexte, 2, 2);
      Console.ResetColor();
      AfficherScore(score);
      System.Threading.Thread.Sleep(2000);

    } //Animation si on gagne


    //--- Afficher l'état des sauvegardes --- o

    static public void AfficherEtatSauvegarde(string etat, string numero) //Affiche l'état des différentes sauvegardes
    {
      if (etat == "Vide")
      {
        Console.WriteLine("La save " + numero + " est vide");
        Console.BackgroundColor = ConsoleColor.Green;
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write("___");
        Console.ResetColor();
        Console.WriteLine();
      }
      else
      {
        Console.WriteLine("La save " + numero + " contient des valeurs");
        Console.BackgroundColor = ConsoleColor.Red;
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write("___");
        Console.ResetColor();
        Console.WriteLine();
      }
    }



    /*================================================================================================================================
    *                                           Fonctions de création de motifs
    ================================================================================================================================*/
    // o
    static public string[][] CreerRectangle(int hauteur, int largeur)
    {
      string[][] tab = new string[hauteur + 2][];
      for (int i = 0; i < hauteur + 2; i++)
      {
        tab[i] = new string[largeur + 2];
        for (int j = 0; j < largeur + 2; j++)
        {
          if (i == 0 || i == hauteur + 1 || j == 0 || j == largeur + 1)
          {
            tab[i][j] = "XXX";
          }
          else
          {
            tab[i][j] = "___";
          }
        }
      }
      return tab;
    } //Créer un rectangle de vide de taille hauteur * largeur

    static public string[][] CreerTriangle(int hauteur)
    {
      string[][] tab = new string[hauteur + 2][];
      for (int i = 0; i < hauteur + 2; i++)
      {
        tab[i] = new string[2 * hauteur + 1];
        for (int j = 0; j < 2 * hauteur + 1; j++)
        {
          tab[i][j] = "XXX";
        }
      }
      for (int i = 0; i < hauteur + 2; i++)
      {
        if (i != 0 || i != hauteur + 1)
        {

          tab[i][hauteur] = "___";
          for (int j = 0; j <= i - 1; j++)
          {

            tab[i][hauteur + j] = "___";
            tab[i][hauteur - j] = "___";
          }

        }
        
      }
      tab[0][hauteur] = "XXX";
      for (int i=0; i < 2 * hauteur + 1; i++)
      {
        tab[hauteur + 1][i] = "XXX";
      }
      return tab;

    } // Créer un triangle de hauteur "hauteur"

    /*================================================================================================================================
    *                                           Fonctions de positionnement des mines et des trésors
    ================================================================================================================================*/
    // o
    static public int PositionnerMines(string[][] motif, int hauteur, int largeur, int nbCases, Random rng, int ligneJ, int colonneJ)
    {

      int nbMines = rng.Next((hauteur - 2) / 2, nbCases / 4);
      for (int k = 0; k < nbMines; k++)
      {
        int ligne = rng.Next(1, hauteur - 1);
        int colonne = rng.Next(1, largeur - 1);
        bool test = false;
        while (test != true)
        {
          ligne = rng.Next(1, hauteur - 1);
          colonne = rng.Next(1, largeur - 1);
          if ((motif[ligne][colonne] == "___"))
          {
            if (motif[ligne][colonne] != " J ")
            {
              if ((motif[ligne][colonne] != " M "))
              {

                test = true;
              }

            }
          }

        }
        motif[ligne][colonne] = " M ";
      }
      return nbMines;
    }  //Positionne les mines sur un rectangle
    static public int PositionnerTrésor(string[][] motif, int hauteur, int largeur, int nbCases, Random rng, int ligneJ, int colonneJ)
    {
      int nbTrésor = rng.Next(1,4);
      for (int k = 0; k < nbTrésor; k++)
      {
        int ligne = rng.Next(1, hauteur - 1);
        int colonne = rng.Next(1, largeur - 1);
        bool test = false;
        while (test != true)
        {
          ligne = rng.Next(1, hauteur - 1);
          colonne = rng.Next(1, largeur - 1);
          if ((motif[ligne][colonne] == "___"))
          {
            if (motif[ligne][colonne] != " J ")
            {
              if ((motif[ligne][colonne] != "XXX"))
              {
                if ((motif[ligne][colonne] != " M "))
                {
                  if ((motif[ligne][colonne] != " T "))
                  {
                    test = true;
                  }
                }
              }

            }
          }
        }
        motif[ligne][colonne] = " T ";

      }
      return nbTrésor;
    } //Positionne les trésors sur un rectangle

    // Fonctionne bien et les mines ainsi que les trésors ne se superposent pas sur le choix (x0,y0) initial du joueur. 


    /*================================================================================================================================
    *                                           Fonctions de tests
    ================================================================================================================================*/
    // o
    static public string FinirJeu(string[][] motif, bool perdu, int nbCases, int hauteur, int largeur, int nbMines)
    {
      int compteur = 0;
      if (perdu == true)
      {
        return "Perdu";
      }
      else
      {

        for (int i = 0; i < hauteur; i++) 
        {
          for (int j = 0; j < largeur; j++)
          {
            if (motif[i][j] != " M ") 

            {
              if (motif[i][j] != "___")
              {
                if (motif[i][j] != " T ")
                {
                  if (motif[i][j] != "XXX")
                  {
                    compteur = compteur + 1;
                  }
                }
              }

            }
          }
        }
        if (compteur == nbCases - nbMines)
        {
          return "Gagné";
        }
        else
        {
          return "Pas Fini";
        }
      }
    } //Fonction de test testant l'état du jeu à chaque tour


    /*================================================================================================================================
    *                                           Fonctions de revelation
    ================================================================================================================================*/

    static public void Reveler(string[][] motif, int nbCases, int hauteur, int largeur, int ligneJu, int colonneJu, ref bool perdu)
    {
      int ligneJ = ligneJu;
      int colonneJ = colonneJu;
      if (motif[ligneJ][colonneJ] == " M ")
      {
        perdu = true;

      }
      else
      {
        if (motif[ligneJ][colonneJ] == " T ")
        {
          motif[ligneJ][colonneJ] = " V ";
        }
        else
        {
          int decompte = 0; // Début du compteur, on n'est ni sur une mine " M " ni sur un trésor " T " ==> On peut donc être : sur "___"/" _ "/" J "
          int[,] voisinsTab = ListerVoisins(motif, hauteur, largeur, ligneJ, colonneJ); //On récupère tous les voisins possibles du nouveau point séléctionner (en comptant les murs etc..)

          for (int nb = 0; nb < voisinsTab.GetLength(0); nb++) // On regarde sur on va plutôt afficher un décompte ou faire de la récursivité
          {

            int x = voisinsTab[nb, 0];
            int y = voisinsTab[nb, 1];
            if (x != -999)
            {
              if (motif[x][y] == " M ") // Si une mine, on augmente de 1
              {
                decompte = decompte + 1;
              }
              if (motif[x][y] == " T ") // Si un trésor, on augmente de 2
              {
                decompte = decompte + 2;
              }
            }

          }

          if (decompte == 0) // Dans ce cas là, on fait preuve de récursivité :
          {
            motif[ligneJ][colonneJ] = " _ ";
            for (int i = 0; i < voisinsTab.GetLength(0); i++)
            {
              int x = voisinsTab[i, 0];
              int y = voisinsTab[i, 1];
              if (x != -999)
              {
                if (motif[x][y] == "___") // Seul cas qui nous interesse. Une case déjà visitée conduirait à une boucle infinie, de même que " J " qui est déjà visité car on était dessus à l'itération d'avant.
                {
                  motif[x][y] = " _ ";
                  Reveler(motif, nbCases, hauteur, largeur, x, y, ref perdu);
                }
              }
            }
          }
          else
          {

            int dec = CalculerDecompte(motif, nbCases, hauteur, largeur, ligneJu, colonneJu, ref perdu);
            motif[ligneJ][colonneJ] = " " + dec + " ";

          }


        }
      }
    } //Revèle les cases selon le mode choisi par l'énoncé

    static public int[,] ListerVoisins(string[][] motif, int hauteur, int largeur, int ligneJ, int colonneJ)
    {
      int[,] voisinsTab;
      int nbVoisins = 8;
      voisinsTab = new int[,] { { ligneJ-1, colonneJ  },
                                    { ligneJ-1, colonneJ -1 },
                                    { ligneJ, colonneJ -1  },
                                    { ligneJ+1, colonneJ -1 },
                                    { ligneJ +1, colonneJ   },
                                    { ligneJ-1, colonneJ +1 },
                                    { ligneJ, colonneJ +1  },
                                    { ligneJ+1, colonneJ +1 }};
      for (int i = 0; i < 8; i++)
      {
        int x = voisinsTab[i, 0];
        int y = voisinsTab[i, 1];
        if (x != -999)
        {
          if (motif[x][y] == "XXX")
          {
            nbVoisins--;
            voisinsTab[i, 0] = -999; //Voisin non valide
            voisinsTab[i, 1] = -999;
          }
        }
      }

      return voisinsTab;
    } //Obtient un tableau des voisins 

    static public int CalculerDecompte(string[][] motif, int nbCases, int hauteur, int largeur, int ligneJu, int colonneJu, ref bool perdu)
    {
      int ligneJ = ligneJu;
      int colonneJ = colonneJu;
      int decompte = 0; // Début du compteur, on n'est ni sur une mine " M " ni sur un trésor " T " ==> On peut donc être : sur "___"/" _ "/" J "
      int[,] voisinsTab = ListerVoisins(motif, hauteur, largeur, ligneJ, colonneJ); //On récupère tous les voisins possibles du nouveau point séléctionner (en comptant les murs etc..)

      for (int nb = 0; nb < voisinsTab.GetLength(0); nb++) // On regarde sur on va plutôt afficher un décompte ou faire de la récursivité
      {

        int x = voisinsTab[nb, 0];
        int y = voisinsTab[nb, 1];
        if (x != -999)
        {
          if (motif[x][y] == " M ") // Si une mine, on augmente de 1
          {
            decompte = decompte + 1;
          }
          if (motif[x][y] == " T ") // Si un trésor, on augmente de 2
          {
            decompte = decompte + 2;
          }
        }
      }
      return decompte;
    }  //Calcul le décompte de chaque case en fonction du tableau de ses voisins

    /*================================================================================================================================
    *                                                           ASCII ART 
    ================================================================================================================================*/
    static public void Exploser()
    {
      var arr1 = new[] {
@" ",
@" ",
@" ",
@" ",
@" ",
@" ",
@" ",
@" ",
@" ",
@" ",
@" ",
@" ",
@" ",
@" ",
@" ",
@" ",
@"                                      * ",
@" ",
@" ",
@" ",
@" ",
@" ",
@" ",
@" ",
@" ",
@" ",
@" ",
@" ",
@"  "};
      var arr2 = new[]{
@" ",
@" ",
@" ",
@" ",
@" ",
@" ",
@" ",
@" ",
@" ",
@" ",
@"                                      **@** ",
@"                                     ***#*@*    ",
@"                                      *****  ",
@" ",
@" ",
@" ",
@" ",
@" ",
@" ",
@" ",
@" ",
@" ",
@" ",
@" ",
@" "};
      var arr3 = new[]{
@" ",
@" ",
@" ",
@" ",
@" ",
@" ",
@" ",
@"                                         * ",
@"                                    *@@#*#***     ",
@"                                   *********** ",
@"                                  *#*@**#@**#@*     ",
@"                                   *@#********   ",
@"                                    *@*##*@** ",
@"                                        * ",
@" ",
@" ",
@" ",
@" ",
@" ",
@" ",
@" "};
      var arr4 = new[]{
@" ",
@" ",
@" ",
@" ",
@" ",
@" ",
@" ",
@" ",
@" ",
@"                                    ********* ",
@"                                  ****@**#*****                    ",
@"                                ***@*#***#*******               ",
@"                                ******@@#****#*#*            ",
@"                               **#******@*#@*#****        ",
@"                                *******#*@#*#****      ",
@"                                ***#****#**@@****   ",
@"                                  *@*****#***@*  ",
@"                                    ********* ",
@" ",
@" ",
@" ",
@" ",
@" ",
@" "};
      var arr5 = new[]{
@" ",
@" ",
@" ",
@" ",
@" ",
@" ",
@" ",
@"                                        * ",
@"                                  *****#@@*****                               ",
@"                                ***@##**@@#****#*                          ",
@"                              *#*#*@@*********@**@*                     ",
@"                             **@**@*@*#@***@**@*****                 ",
@"                             **@*#*@**@***#**@***@**              ",
@"                            ***@**@@***@***#****@**@*          ",
@"                             ***#***@#*#@@**@@*@****        ",
@"                             **#@**#*******#**@*@@**     ",
@"                              **#*#****@***@##*##**   ",
@"                                ****@***@*#*@****  ",
@"                                  **@**#*##**** ",
@"                                        * ",
@" ",
@" ",
@" ",
@" "};
      var arr6 = new[]{
@" ",
@" ",
@" ",
@" ",
@" ",
@" ",
@"                                 *********** ",
@"                               ***#**#**#*@**@#***                               ",
@"                             ****#*#@*@#@***@*@**#**                             ",
@"                            *#***@****#@*@**#********                            ",
@"                           *@*#********#*******@#*@@**                        ",
@"                          **@****#@@*@********@#@@**##*                    ",
@"                          ***@@#**@**#*******#*****##**                 ",
@"                         ****@**@@*@@***@@**#@*#***#****             ",
@"                          **#*****#*******@**#**#****#*           ",
@"                          *@@@#@***#****@****#@@***@@#*        ",
@"                           ******@***#**##@*@**@@@#*@*      ",
@"                            *#*****@@#**@*****#******    ",
@"                             *****@#*@*#@**@**@@****  ",
@"                               ***@***@*******@*** ",
@"                                   *********** ",
@" ",
@" ",
@" ",
@" "};
      var arr7 = new[]{
@" ",
@" ",
@" ",
@"                                        * ",
@"                                *****@*@ *@@*@*** ",
@"                             **#*  ****@ *****  ****                             ",
@"                           *@* *@  @#*** *#**#  *# *#*                           ",
@"                          *@ ** **  ***@ *@*@  *# @* #*                          ",
@"                        **# * ** *  @##* @@@*  * #* # *#*                        ",
@"                        ** * @ @  @  **@ #**  #  # # * **                        ",
@"                       ** @ @ * ** # *** *#* * *@ * # # **                      ",
@"                       ** * * * * * * ** ** * * * * * # **                   ",
@"                      ** * @ * * * # * * * * * * @ # * @ #*               ",
@"                       ** # * * * * * ** ** * * # * * * **             ",
@"                       *# # * * #* * *** *** @ ** * @ * **          ",
@"                        *# * * @  @  @*@ ***  #  @ * @ @*        ",
@"                        *#@ * #* #  #*@@ ***#  * @# * ***     ",
@"                          ** ** **  *#@* ***@  @@ #@ **    ",
@"                           *@# @*  **@#* #*@@#  #* *@*  ",
@"                             **@*  *@@** #*@**  **** ",
@"                                ***#**** @@****** ",
@"                                        * ",
@" ",
@" "};
      var arr8 = new[]{
@" ",
@" ",
@"                                  ****** ****** ",
@"                              **  *##*** ******  **                              ",
@"                           *  **   **#** *#***   *#  *                           ",
@"                         *  *  **  **@*# *@***  **  *  *                         ",
@"                       *  * ** **  ***** *#***  @* ** @  *                       ",
@"                      * ** @ *# **  @#*# **#*  ** ** @ *# *                      ",
@"                     * * ** * ** #  *#** ***@  * #* * @* @ *                    ",
@"                    * #* # * * @  *  **@ ***  @  * @ @ * *# *                ",
@"                    * * * # * * ** * *#* *** * #* * * * * @ *                 ",
@"                    * # @ * # * @ * * #* #* * * # * * * * # *                ",
@"                   * * * # * * * # * * * * * # # @ * * * * * *              ",
@"                    * # @ * * @ * # * ** ** # * @ * @ * * * *                 ",
@"                    * @ * * * # *@ * **@ @*# @ ** * * * * * *            ",
@"                    * ** * * @ *  *  *@* ***  *  # * * @ *# *         ",
@"                     * # ** @ ** @  **#* #@@@  * *# # *# * *       ",
@"                      * @* @ *# **  *#*@ #***  ** *# * ** *     ",
@"                       *  * *# @*  @@*@* ***@*  @* ** *  *   ",
@"                         *  *  **  *##*# #*##*  **  *  *  ",
@"                           *  **   *@*** @#*@@   **  * ",
@"                              **  ****** ******  ** ",
@"                                  ****** ****** ",
@" "};
      var arr9 = new[]{
@"                                        * ",
@"                               ****@**@@ ********* ",
@"                           ****   **##*# ***#**   ****                           ",
@"                         ***  **  ***#@* **##**  *@  @**                         ",
@"                       *@  #  **   ***** ***@*   **  *  **                       ",
@"                     *** *  *  **  @#*** ****@  *@  *  # ***                     ",
@"                    *@ *  # #* *@  #*##* @*@**  #* @* *  * **                    ",
@"                   ** * #* * ** **  ***# ****  *@ #* * *@ * **                   ",
@"                  *@ # # @@ @ @* *  ***@ ****  * @* * #* * * @*                  ",
@"                *# @ ** @ * * *  *  *** ***  *  * @ * # #@ @ **                 ",
@"                 *# @ * * * * * ** @ *** *@* * @* * * * * * # #*                 ",
@"                 ** * * @ * * * * * * ** @* # @ * # @ * * * * **                 ",
@"                ** * * @ * * * * @ # @ * * # * * * * # * * * @ **                ",
@"                 ** * * * * # * * # * ** *# @ * @ * * # @ * * @*                 ",
@"                 *# @ * @ * @ # ** * @#@ *#* * ** # * @ * # * **                 ",
@"                 ** # ** * # * *  #  ### ***  @  * * # # *@ * @*                 ",
@"                  *@ * * ## * #* #  **** ****  * @@ @ *# * # **                  ",
@"                   ** # @* * @* **  #@** #**@  #* *# @ ** * **                   ",
@"                    *@ #  @ ** #*  *@*** *@*@*  #* ** *  * **                    ",
@"                     **@ @  *  **  ***** *****  #*  *  * ***                     ",
@"                       *#  *  *@   ####* **#**   *#  #  **                       ",
@"                         **#  #*  #****@ *#**@*  *#  *#*                        ",
@"                           ***@   *#@##@ *@****   ****                       ",
@"                               ********@ @#*@*****               "          };
      var arr10 = new[]{
@"                            **   ------- -------   ** ",
@"                         **  --   ------ ------   --  ** ",
@"                      ** --  --   ------ ------   --  -- ** ",
@"                    ** -- --  --  ------ ------  --  -- -- ** ",
@"                   * -- -  -  --   ----- -----   --  -  - -- * ",
@"                 ** - -- -  -  --  ----- -----  --  -  - -- - ** ",
@"                *  - - -  - -- --  ----- -----  -- -- -  - - -  * ",
@"               *  - - - -- - -- --  ---- ----  -- -- - -- - - -  * ",
@"               * - - - - -- - -- -  ---- ----  - -- - -- - - - - * ",
@"              * - - - -- - - - -  -  --- ---  -  - - - - -- - - - * ",
@"              * - - - - - - - - -- - --- --- - -- - - - - - - - - * ",
@"              * - - - - - - - - - - - -- -- - - - - - - - - - - - * ",
@"             * - - - - - - - - - - - - - - - - - - - - - - - - - - * ",
@"              * - - - - - - - - - - - -- -- - - - - - - - - - - - * ",
@"              * - - - - - - - - -- - --- --- - -- - - - - - - - - * ",
@"              * - - - -- - - - -  -  --- ---  -  - - - - -- - - - * ",
@"               * - - - - -- - -- -  ---- ----  - -- - -- - - - - * ",
@"               *  - - - -- - -- --  ---- ----  -- -- - -- - - -  * ",
@"                *  - - -  - -- --  ----- -----  -- -- -  - - -  * ",
@"                 ** - -- -  -  --  ----- -----  --  -  - -- - ** ",
@"                   * -- -  -  --   ----- -----   --  -  - -- * ",
@"                    ** -- --  --  ------ ------  --  -- -- ** ",
@"                      ** --  --   ------ ------   --  -- **  ",
@"                         **  --   ------ ------   --  **       "          };
      var arr11 = new[]{
@"                      **--  --   ------- -------   --  --** ",
@"                    **-  --  --   ------ ------   --  --  -** ",
@"                  *-- -- --  --   ------ ------   --  -- -- --* ",
@"                **- -- -- --  --  ------ ------  --  -- -- -- -** ",
@"               *-- - -- -  -  --   ----- -----   --  -  - -- - --* ",
@"              *- -- - -- -  -  --  ----- -----  --  -  - -- - -- -* ",
@"             *- -  - - -  - -- --  ----- -----  -- -- -  - - -  - -* ",
@"            *- -  - - - -- - -- --  ---- ----  -- -- - -- - - -  - -* ",
@"            *- - - - - - -- - -- -  ---- ----  - -- - -- - - - - - -* ",
@"           *- - - - - -- - - - -  -  --- ---  -  - - - - -- - - - - -* ",
@"           *- - - - - - - - - - -- - --- --- - -- - - - - - - - - - -* ",
@"           *- - - - - - - - - - - - - -- -- - - - - - - - - - - - - -* ",
@"          *- - - - - - - - - - - - - - - - - - - - - - - - - - - - - -* ",
@"           *- - - - - - - - - - - - - -- -- - - - - - - - - - - - - -* ",
@"           *- - - - - - - - - - -- - --- --- - -- - - - - - - - - - -* ",
@"           *- - - - - -- - - - -  -  --- ---  -  - - - - -- - - - - -* ",
@"            *- - - - - - -- - -- -  ---- ----  - -- - -- - - - - - -* ",
@"            *- -  - - - -- - -- --  ---- ----  -- -- - -- - - -  - -* ",
@"             *- -  - - -  - -- --  ----- -----  -- -- -  - - -  - -* ",
@"              *- -- - -- -  -  --  ----- -----  --  -  - -- - -- -* ",
@"               *-- - -- -  -  --   ----- -----   --  -  - -- - --* ",
@"                **- -- -- --  --  ------ ------  --  -- -- -- -** ",
@"                  *-- -- --  --   ------ ------   --  -- -- --* ",
@"                    **-  --  --   ------ ------   --  --  -**      "        };
      var arr12 = new[]{
@"                  *  -  --  --   ------- -------   --  --  -  * ",
@"                *  -  -  --  --   ------ ------   --  --  -  -  * ",
@"              ** - -- -- --  --   ------ ------   --  -- -- -- - ** ",
@"             * -  - -- -- --  --  ------ ------  --  -- -- -- -  - * ",
@"            * - -- - -- -  -  --   ----- -----   --  -  - -- - -- - * ",
@"           * - - -- - -- -  -  --  ----- -----  --  -  - -- - -- - - * ",
@"          * - - -  - - -  - -- --  ----- -----  -- -- -  - - -  - - - * ",
@"         * - - -  - - - -- - -- --  ---- ----  -- -- - -- - - -  - - - * ",
@"        ** - - - - - - - -- - -- -  ---- ----  - -- - -- - - - - - - - ** ",
@"        * - - - - - - -- - - - -  -  --- ---  -  - - - - -- - - - - - - * ",
@"        * - - - - - - - - - - - -- - --- --- - -- - - - - - - - - - - - * ",
@"        * - - - - - - - - - - - - - - -- -- - - - - - - - - - - - - - - * ",
@"       * - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - * ",
@"        * - - - - - - - - - - - - - - -- -- - - - - - - - - - - - - - - * ",
@"        * - - - - - - - - - - - -- - --- --- - -- - - - - - - - - - - - * ",
@"        * - - - - - - -- - - - -  -  --- ---  -  - - - - -- - - - - - - * ",
@"        ** - - - - - - - -- - -- -  ---- ----  - -- - -- - - - - - - - ** ",
@"         * - - -  - - - -- - -- --  ---- ----  -- -- - -- - - -  - - - * ",
@"          * - - -  - - -  - -- --  ----- -----  -- -- -  - - -  - - - * ",
@"           * - - -- - -- -  -  --  ----- -----  --  -  - -- - -- - - * ",
@"            * - -- - -- -  -  --   ----- -----   --  -  - -- - -- - * ",
@"             * -  - -- -- --  --  ------ ------  --  -- -- -- -  - * ",
@"              ** - -- -- --  --   ------ ------   --  -- -- -- - ** ",
@"                *  -  -  --  --   ------ ------   --  --  -  -  *      "    };
      var arr13 = new[]{
@"              *                                                   * ",
@"            *                                                       * ",
@"           *                                                         * ",
@"         *                                                             * ",
@"        *                                                               * ",
@"       *                                                                 * ",
@"       *                                                                 * ",
@"      *                                                                   * ",
@"     *                                                                     * ",
@"     *                                                                     * ",
@"     *                                                                     * ",
@"     *                                                                     * ",
@"    *                                                                       * ",
@"     *                                                                     * ",
@"     *                                                                     * ",
@"     *                                                                     * ",
@"     *                                                                     * ",
@"      *                                                                   * ",
@"       *                                                                 * ",
@"       *                                                                 * ",
@"        *                                                               * ",
@"         *                                                             * ",
@"           *                                                         * ",
@"            *                                                       *      "      };
      var arr14 = new[]{
@"          *                                                           * ",
@"        **                                                             ** ",
@"       *                                                                 * ",
@"      *                                                                   * ",
@"     *                                                                     * ",
@"    *                                                                       * ",
@"   *                                                                         * ",
@"   *                                                                         * ",
@"  *                                                                           * ",
@"  *                                                                           * ",
@"  *                                                                           * ",
@"  *                                                                           * ",
@" *                                                                             * ",
@"  *                                                                           * ",
@"  *                                                                           * ",
@"  *                                                                           * ",
@"  *                                                                           * ",
@"   *                                                                         * ",
@"   *                                                                         * ",
@"    *                                                                       * ",
@"     *                                                                     * ",
@"      *                                                                   * ",
@"       *                                                                 * ",
@"        **                                                             **      "   };
      var arr15 = new[]{
@"      *                                                                   * ",
@"     *                                                                     * ",
@"    *                                                                       * ",
@"   *                                                                         * ",
@"  *                                                                           * ",
@" *                                                                             * ",
@"* ",
@"* ",
@" ",
@" ",
@" ",
@" ",
@" ",
@" ",
@" ",
@"* ",
@"* ",
@" *                                                                             * ",
@"  *                                                                           * ",
@"   *                                                                         * ",
@"    *                                                                       * ",
@"     *                                                                     *       "          };
      var arr16 = new[]{
@"  **                                                                         ** ",
@" *                                                                             * ",
@"* ",
@" ",
@" ",
@" ",
@" ",
@" ",
@" ",
@" ",
@" ",
@" ",
@" ",
@" ",
@" ",
@" ",
@" ",
@" ",
@" ",
@" ",
@" ",
@" ",
@"*                                                                                                                      "};
      var arr17 = new[]{
@"*                                                                              * ",
@" ",
@" ",
@" ",
@" ",
@" ",
@" ",
@" ",
@" ",
@" ",
@" ",
@" ",
@" ",
@" ",
@" ",
@" ",
@" ",
@" ",
@" ",
@" ",
@" ",
@" ",
@" ",
@" ",
@" ",
@" ",
@"    "};
      Console.WindowWidth = 160;
      Console.WriteLine("\n\n");
      var maxLength = arr1.Aggregate(0, (max, line) => Math.Max(max, line.Length));
      var x = Console.BufferWidth / 2 - maxLength / 2;


      DessinerConsole(arr1, 0, 2);
      System.Threading.Thread.Sleep(4);
      Console.Clear();
      DessinerConsole(arr2, 0, 2);
      System.Threading.Thread.Sleep(4);
      Console.Clear();
      DessinerConsole(arr3, 0, 2);
      System.Threading.Thread.Sleep(4);
      Console.Clear();
      DessinerConsole(arr4, 0, 2);
      System.Threading.Thread.Sleep(4);
      Console.Clear();
      DessinerConsole(arr5, 0, 2);
      System.Threading.Thread.Sleep(4);
      Console.Clear();
      DessinerConsole(arr6, 0, 2);
      System.Threading.Thread.Sleep(4);
      Console.Clear();
      DessinerConsole(arr7, 0, 2);
      System.Threading.Thread.Sleep(4);
      Console.Clear();
      DessinerConsole(arr8, 0, 2);
      System.Threading.Thread.Sleep(4);
      Console.Clear();
      DessinerConsole(arr9, 0, 2);
      System.Threading.Thread.Sleep(4);
      Console.Clear();
      DessinerConsole(arr10, 0, 2);
      System.Threading.Thread.Sleep(4);
      Console.Clear();
      DessinerConsole(arr11, 0, 2);
      System.Threading.Thread.Sleep(4);
      Console.Clear();
      DessinerConsole(arr12, 0, 2);
      System.Threading.Thread.Sleep(4);
      Console.Clear();
      DessinerConsole(arr13, 0, 2);
      System.Threading.Thread.Sleep(4);
      Console.Clear();
      DessinerConsole(arr14, 0, 2);
      System.Threading.Thread.Sleep(4);
      Console.Clear();
      DessinerConsole(arr15, 0, 2);
      System.Threading.Thread.Sleep(4);
      Console.Clear();
      DessinerConsole(arr16, 0, 2);
      System.Threading.Thread.Sleep(4);
      Console.Clear();
      DessinerConsole(arr17, 0, 2);
      System.Threading.Thread.Sleep(4);
      Console.Clear();



    } //Animation d'explosion. o

    /*================================================================================================================================
        *                                                        Outils de Versionning
        ================================================================================================================================*/

    static public string[] LireAnciennePositionDeJeu(int numero) //Lis la dernière position jouée avant d'avoir fermé le jeu
    {
      string[] positions = File.ReadAllLines(@"OldPosition" + numero + ".txt");
      return positions;
    }

    static public void EcrireAnciennePositionDeJeu(int x, int y, int numero) // Permet de sauvegarder à chaque tour où se trouve le dernier coup joué par le joueur. 
    {
      string h = @"OldPosition";
      h += numero;
      h += ".txt";
      File.Delete(h);
      EcrireUneLigne(x.ToString(), h);
      EcrireUneLigne(y.ToString(), h);

    }

    static public string[][] GererVersionning(string choix, string saveTxt, ref string etat, string[] etatsTab) //Renvoie la grilleUU et change si on écrase les données, les datas de la sauvegarde.
    {
      if (choix == "Charger")
      {
        string[][] grilleUU = LireSauvegarde(saveTxt);
        return grilleUU;
      }
      else
      {
        string h = SupprimerUneSauvegarde(saveTxt, etatsTab);
        etat = h;
        string[][] grilleUU = LireSauvegarde(saveTxt);
        return grilleUU;
      }



    }
    static public void GererUsername() // Gère la notion Username, et créer un fichier si on n'a pas encore renseigné dans nom d'utilisateur. 
    {
      if (!System.IO.File.Exists(@"NomUser.txt"))
      {
        CentrerTxt("Merci de nous indiquer un pseudo pour commencer ", 1);
        string x = Console.ReadLine();
        EcrireUneLigne(x, @"NomUser.txt");
        Console.Clear();
      }
    }
    static public void TesterVersionning() // Test si tous les fichiers de versionning sont présents. Dans le cas inverse, les génère avec des paramètres
                                           //considérés par défauts, explicités dans la Documentation Technique
    {

      string save1 = @"save1.txt";
      string save2 = @"save2.txt";
      string save3 = @"save3.txt";
      string save4 = @"save4.txt";
      string scoreSAVER = @"ScoreSaver.txt";
      string etatsSAVER = @"etatsSaver.txt";
      string motifsSAVE = @"motifsSave.txt";
      string[][] grilleTest = CreerRectangle(20, 20);

      if (!System.IO.File.Exists(save1))
      {
        EcrireUneLigne("V", save1);
        EcrireUneLigne(20.ToString(), save1);
        EcrireUneLigne(20.ToString(), save1);
        EcrireUneLigne(1.ToString(), save1);
        EcrireUneSauvegarde(grilleTest, 1, "Vide", 20, 20, save1);

      }
      if (!System.IO.File.Exists(save2))
      {
        EcrireUneLigne("V", save2);
        EcrireUneLigne(20.ToString(), save2);
        EcrireUneLigne(20.ToString(), save2);
        EcrireUneLigne(1.ToString(), save2);
        EcrireUneSauvegarde(grilleTest, 1, "Vide", 20, 20, save2);

      }
      if (!System.IO.File.Exists(save3))
      {
        EcrireUneLigne("V", save3);
        EcrireUneLigne(20.ToString(), save3);
        EcrireUneLigne(20.ToString(), save3);
        EcrireUneLigne(1.ToString(), save3);
        EcrireUneSauvegarde(grilleTest, 1, "Vide", 20, 20, save3);

      }
      if (!System.IO.File.Exists(save4))
      {
        EcrireUneLigne("V", save4);
        EcrireUneLigne(20.ToString(), save4);
        EcrireUneLigne(20.ToString(), save4);
        EcrireUneLigne(1.ToString(), save4);
        EcrireUneSauvegarde(grilleTest, 1, "Vide", 20, 20, save4);

      }
      
      if (!System.IO.File.Exists(etatsSAVER))
      {
        using (System.IO.StreamWriter writer = System.IO.File.CreateText(etatsSAVER))
        {
          writer.WriteLine("Vide");
          writer.WriteLine("Vide");
          writer.WriteLine("Vide");
          writer.WriteLine("Vide");
        }

      }
      if (!System.IO.File.Exists(motifsSAVE))
      {
        using (System.IO.StreamWriter writer = System.IO.File.CreateText(motifsSAVE))
        {
          writer.WriteLine("rectangle");
          writer.WriteLine("rectangle");
          writer.WriteLine("rectangle");
          writer.WriteLine("rectangle");
        }

      }

    }

    static public string SupprimerUneSauvegarde(string saveTxt, string[] etatsTab)
    {
      try
      {



        File.Delete(saveTxt);
        Console.WriteLine();
        Console.WriteLine("Sauvegarde supprimée !");
        TesterVersionning(); //On recrée une sauvegarde vide.
        MettreAJourDesEtats(etatsTab);
        return "Vide";


      }
      catch (IOException ioExp)
      {
        Console.WriteLine(ioExp.Message);
        return "Error";
      }
    } //return "Vide" et supprimer une sauvegarde

    static public void EcrireUneSauvegarde(string[][] grille, int score, string etat, int hauteur, int largeur, string saveTxt)
    {
      File.Delete(saveTxt);
      string txt = saveTxt.Substring(0, 5);
      EcrireUneLigne("P", saveTxt);
      EcrireUneLigne(hauteur.ToString(), saveTxt);
      EcrireUneLigne(largeur.ToString(), saveTxt);
      EcrireUneLigne(score.ToString(), saveTxt);

      for (int i = 0; i < grille.Length; i++)
      {

        for (int j = 0; j < grille[1].Length; j++)
        {

          EcrireUneLigne(grille[i][j], saveTxt);
        }
      }
    } //Ecris une sauvegarde de la partie dans le fichier sélectionné.

    static public void EcrireScore(int score)
    {
      EcrireUneLigne(score.ToString(), @"scoreSAVER.txt");
    } //Rajoute un score dans le document scoreSAVER.txt

    static public void EcrireMotifsSauvegarde(int numeroSauvegarde, string nomMotif) // Sauvegarde dans motifsSave.txt les motifs de chaque save. Par défaut : "rectangle"
    {
      string[] lines = File.ReadAllLines(@"motifsSave.txt");
      lines[numeroSauvegarde] = nomMotif;
      File.Delete(@"motifsSave.txt");
      for (int i = 0; i < 4; i++)
      {
        EcrireUneLigne(lines[i], @"motifsSave.txt");
      }
    }

    static public void AfficherEtatSauvegarde2(string etat1, string etat2, string etat3, string etat4)
    {
      AfficherEtatSauvegarde(etat1, "1");
      AfficherEtatSauvegarde(etat2, "2");
      AfficherEtatSauvegarde(etat3, "3");
      AfficherEtatSauvegarde(etat4, "4");
    } //Affiche l'état des sauvegardes

    static public string[][] LireSauvegarde(string saveTxt) //Lis le fichier txt contenant la sauvegarde et renvoie la grille avec toutes les informations.
    {
      string[] lines = File.ReadAllLines(saveTxt); 

      int hauteur = int.Parse(lines[1]);
      int largeur = int.Parse(lines[2]);
      string[][] grilleUU = new string[hauteur + 1][];
      string[] codeU = new string[] { lines[0], lines[1], lines[2], lines[3] };
      grilleUU[0] = codeU;

      for (int i = 1; i < hauteur + 1; i++)
      {
        grilleUU[i] = new string[largeur];
      }
      for (int i = 4; i < (hauteur * largeur)+4; i++)
      {
        int h = ((i - 4) / largeur) + 1;
        int f = (i - 4) % largeur;
        grilleUU[((i - 4) / (largeur)) +1][(i - 4) % (largeur)] = lines[i];
      }


      return grilleUU;

    }

    static public string[][] ExtraireGrille(string[][] grilleUU) // Extrait la grille de jeu de GrilleUU
    {
      int n = grilleUU.GetLength(0);
      string[][] res = new string[n - 1][];
      for (int i = 1; i < grilleUU.GetLength(0); i++)
      {
        res[i - 1] = grilleUU[i];
      }
      return res;
    }

    static public string[] ExtraireCodeUtile(string[][] grilleUU) // Extrait le Code_Information (4 première lignes du .txt)
    {
      string[] res = grilleUU[0];

      return res;
    }

    static public void MettreAJourDesEtats(string[] etatsTab) // Mets à jour les états suivant un tableau, dans le fichier etatsSaver.txt
    {
      File.Delete(@"etatsSaver.txt");

      for (int j = 0; j < etatsTab.GetLength(0); j++)
      {
        EcrireUneLigne(etatsTab[j], @"etatsSaver.txt");
      }
    }

    static public void EcrireUneLigne(string valeur, string saveTxt) // Écrire une ligne dans un fichier texte saveTxt, avec la valeur "valeur"
    {
      using (StreamWriter writer1 = new StreamWriter(saveTxt, append: true))
      {

        writer1.WriteLine(valeur);
      }
    }

    /*================================================================================================================================
     *                                                           A FAIRE
     =================================================================================================================================*/
    /* 
     
      
     */


  } // Class Program
}//Namespace
