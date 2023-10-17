using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Globalization;
// ------------------------------------------------- TÄSTÄ YLÖSPÄIN EI TARVITSE TEHDÄ MITÄÄN ------------------------------------------------

// Mittareiden filtterit - käytä aina numerojärjestyksessä, jätä käyttämättömät tyhjiksi
var filter1 = "D_Kausi[Vuosi_nro] = 2023";
var filter2 = "D_Kausi[Vuosineljannes_nro] = 1";
var filter3 = "" ;

// Tulosten tallennuspaikka:
var filePath = "C:\\Users\\patla\\Desktop\\OP3_measure.csv"; // <-- HUOM VAIHDA TÄHÄN OMA POLKU!
// Mittarit sisältävä taulu:
var measureTable = "Mittarit"; // <-- Mittarit pitäisi olla aina Mittarit nimisessä taulussa, mutta jos jossain muualla, vaihda nimi

// ------------------------------------------------- TÄSTÄ ALASPÄIN EI TARVITSE TEHDÄ MITÄÄN ------------------------------------------------
List<string> resultList = new List<string>();
List<string> numberedList = new List<string>();
List<string> errorList = new List<string>();

// Tehdään tietojoukon mittareista lista
var measureNames = Model.Tables[measureTable].Measures.Select(measure => measure.Name).ToList();
var Counter = 1;

// Lasketaan mittarit ja lisätään mittarin ryhmittely, nimi ja tulos listaan
foreach (var measureName in measureNames) 
{   
    try
    {
        var measureResult = 
            EvaluateDax(
                "CALCULATE( " +
                    Model.Tables[measureTable].Measures[measureName].Expression + 
                    Filters(filter1, filter2, filter3) +
                ")"
            );
        var resultString = "null";
        var measureFolder = "null";
        var measureGrouping = "null";
        var measureFormatString = "null";

        measureFormatString = Model.Tables[measureTable].Measures[measureName].FormatString;

        // Formatoidaan helpommin luettavaan muotoon
        if ( measureResult != null ) 
        {
            // Prosentit
            if ( measureFormatString.EndsWith("%") )
            {
                double measureDouble = Convert.ToDouble(measureResult);
                CultureInfo culture = CultureInfo.GetCultureInfo("fi-FI");
                resultString = measureDouble.ToString("P1");
                measureFormatString = "%";
            }
            // Muut luvut
            else 
            {
                resultString = String.Format("{0:0,0.#}", measureResult);
                measureFormatString = "Double";
            }
        } 
        // Tunnistamattomat
        else 
        {
            measureFormatString = "Unknown";
        }

        // Mittarin ryhmittely
        measureFolder = Model.Tables[measureTable].Measures[measureName].DisplayFolder;
        string[] hierarchies = measureFolder.Split("\\");
        measureGrouping = hierarchies[hierarchies.Length - 1];

        // Lisätään tulokset tuloslistaan
        resultList.Add(Counter + ";" + measureGrouping + ";" + measureFormatString + ";" +  measureName + ";" + resultString );
        
    }
    // Jos mittareissa on virheitä, laitetaan ne omaan listaan
    catch (Exception e )
    {
        errorList.Add(Counter + " " + measureName + ", Error: " + e);
    }
    Counter = Counter + 1;
}
// Näytetään tulokset
resultList.Output();
errorList.Output();

// Tehdään csv tiedosto
using (var writer = new StreamWriter(filePath, false, Encoding.UTF8))
{
    // Kirjoitetaan otsikot
    writer.WriteLine("Indeksi;Ryhmittely;Formaatti;Mittari;Arvo");

    // Kirjoitetaan rivit
    foreach (var measure in resultList)
    {
        writer.WriteLine(measure);
    }
}

("CSV file created successfully.").Output();

// Funktio händläämään filttereiden käyttöä
static string Filters(string filter1, string filter2, string filter3)
{
    if (filter3 != "")
    {
        return "," + filter1 + "," + filter2 + "," + filter3;
    } 
    else if (filter2 != "")
    {
        return "," + filter1 + "," + filter2;
    }
    else if (filter1 != "")
    {
        return "," + filter1;
    }
    else {return "";}
}