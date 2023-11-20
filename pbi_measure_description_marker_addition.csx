using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Globalization;
using System.Text.RegularExpressions;

/*
    Valitse TOM Explorerista raporteilla käytössä olevat mittarikenttäparametritaulut ja aja skripti ----->
    Valittujen kenttäparametritaulujen sisältämien mittareiden descriptionin alkuun lisätään "∑",
    mikäli se puuttuu.
*/

List<string> sigmaMeasureList = new List<string>();

if(Selected.Tables.Count == 0) throw new Exception("No tables selected!");

var _selected_tables = Selected.CalculatedTables;
var measureList = Model.Tables["Mittarit"].Measures.Select(measure => measure).ToList();

foreach(var table in _selected_tables)
{
    // hajotetaan DAX -ilmaisu objekteiksi
    var daxExpression = table.Expression;
    string[] daxParts = daxExpression.Split(",");
    // käydään objektit läpi, poimitaan kenttäparametritaulujen DAX -objektit listaan
    foreach(var p in daxParts)
    {
        if(p.Contains("NAMEOF"))
        {
            sigmaMeasureList.Add(p);
        }
    }
    // regex kaava jolla mittari poimitaan objektista
    string pattern = @"\[(.*?)\]";
    // verrataan kenttäparametritauluissa olevia mittareita Mittarit -taulun mittareihin
    foreach(var s in sigmaMeasureList)
    {
        Match match = Regex.Match(s, pattern);
        foreach(var m in measureList)
        {
            if( m.Name == match.Groups[1].Value)
            {   
                // jos vertailussa löytyy sama mittari, jolla "∑" -puuttuu, lisätään se descriptionin alkuun
                if(m.Description.StartsWith("∑") == false)
                {
                m.Description = "∑ " + m.Description;                    
                }
            }
        }
    }
}

Output("∑:t lisätty");
