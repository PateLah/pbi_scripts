using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Globalization;
using System.Text.RegularExpressions;
// ------------------------------------------------- TÄSTÄ YLÖSPÄIN EI TARVITSE TEHDÄ MITÄÄN ------------------------------------------------


// Mittarit sisältävä taulu:
string measureTable = "Mittarit";
// Nimeä uusi kenttäparametritaulu
string fieldParameterTableName = "P_Mittarit_test";

// VALITSE MITTARIT TOM Explorerista, tehdään kenttäparametritauluun valintajärjestyksessä ----->

// ------------------------------------------------- TÄSTÄ ALASPÄIN EI TARVITSE TEHDÄ MITÄÄN ------------------------------------------------

// Tehdään valituista mittareista
var selectedMeasures = Selected.Measures.Select(measure => measure).ToList();
string fpt = "";
int measureListLen= selectedMeasures.Count();
var counter = 1;

// Tuottaa mittariosuuden
foreach( var m in selectedMeasures )
    {
       string[] hierarchies = m.DisplayFolder.Split("\\");
       string measureGrouping = hierarchies[hierarchies.Length - 1];
       if( counter != measureListLen )
        {
            fpt = fpt + "(\"" + ApplyDefaultTranslation(m) + "\", NAMEOF(\'" + measureTable + "\'[" + m.Name + "]), " + counter + ", \"" + measureGrouping + "\"),\n";
        }
        else
        {
            fpt = fpt + "(\"" + ApplyDefaultTranslation(m) + "\", NAMEOF(\'" + measureTable + "\'[" + m.Name + "]), " + counter + ", \"" + measureGrouping + "\")";
        }
        counter = counter + 1;
       
    };

// Tuottaa kenttäparametritaulun ja paketoi mittarit siihen
var fieldParameterTable = Model.AddCalculatedTable( fieldParameterTableName, 
    "{\n" + 
    fpt
    + "\n}"
);


// Käännösfunktio
string ApplyDefaultTranslation(ITranslatableObject obj)
{
    if(true)
    {
        // Etsii mittarinimen lopusta yy[y]-yy[y] tyyppistä kaavaa, jonka muuttaa (yy[y]/yy[y])
        string pattern = @"_(\w{2,3})-(\w{2,3}$)";        

        string technicalName = obj.Name;
        // Sulutetaan loppuliitteitä
        technicalName = Regex.Replace(technicalName, pattern, " ($1/$2)");
        // Plussat
        technicalName = technicalName.Replace("plus", "+");
        // Välilyönnit
        technicalName = technicalName.Replace("_", " ");
        technicalName = technicalName.Replace("-", " ");
        // Taulujen etuliite
        technicalName = Regex.Replace(technicalName, @"^([A-Z])\s", "$1_");
        // Prosentit
        technicalName = Regex.Replace(technicalName, @"(\w+)prosentti", "$1-%");
        technicalName = Regex.Replace(technicalName, @"prosentti(\w+)", "%-$1");
        // Summa
        technicalName = technicalName.Replace(" summa", "");
        // Organisaation ääkköset
        string aoReplaced = technicalName.Replace("yksikko", "yksikkö");
        aoReplaced = aoReplaced.Replace("ryhma", "ryhmä");
        aoReplaced = aoReplaced.Replace("iinteisto", "iinteistö");  // Kiinteistö/kiintestö
        aoReplaced = aoReplaced.Replace("aynti", "äynti");          // Käynti/käynti
        aoReplaced = aoReplaced.Replace("Ika", "Ikä");
        aoReplaced = aoReplaced.Replace("aytetty", "äytetty");      // Käytetty/käytetty
        aoReplaced = aoReplaced.Replace("asittely", "äsittely");    // Käsittely
        aoReplaced = aoReplaced.Replace("aytto", "äyttö");
        aoReplaced = aoReplaced.Replace("Pyha", "Pyhä");
        aoReplaced = aoReplaced.Replace("Paiva", "Päivä");
        aoReplaced = aoReplaced.Replace("paiva", "päivä");
        aoReplaced = aoReplaced.Replace("pitka", "pitkä");
        aoReplaced = aoReplaced.Replace("neljannes", "neljännes");
        
        string translatedName = aoReplaced;
        return translatedName;
        
    }
}
