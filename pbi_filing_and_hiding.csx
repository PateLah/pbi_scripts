using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Globalization;
using System.Text.RegularExpressions;

/*

*** Piilottaa F_, M_, P_, T_ -alkuiset taulut
*** Piilottaa kaikki Dim_ -alkuiset sarakkeet
*** Kansioi kaikki Organisaatio -taulujen sarakkeet
*** Kansioi kaikki TOM explorerista valittujen taulujen sarakkeet taso* -kansioihin -->

Ei vaadi mitään muuttujia
---------------------------------------------------------------------------------------------
*/

var _all_tables = Model.Tables;
foreach(var table in _all_tables)
{
//  Piilotetaan fakta-, parametri- ja metatietotaulut
    var t_n = table.Name;
    string[] tableNameParts = t_n.Split('_');
    string tablePrefix = tableNameParts[0];
    if (tablePrefix == "F" || tablePrefix == "M" || tablePrefix == "P" || tablePrefix == "T")
    {
        table.IsHidden = true;
    }

    var cols = table.Columns.ToList();

    foreach(var c in cols)
    {
        // Piilotetaan kaikista tauluista Dim_ -sarakkeet
        var c_n = c.Name;
        string[] parts = c_n.Split('_');
        string groupName = parts[0];
        if (groupName == "Dim")
        {
            c.IsHidden = true;
        }
    }
}

// Organisaatiot ja kuntadimensiot kansioidaan
var _org_tables = Model.Tables.Where(t => t.Name.StartsWith("L_Organisaatio") || t.Name.StartsWith("D_Organisaatio") || t.Name.StartsWith("L_Kunta")).ToList();

foreach(var table in _org_tables)
{

    // Sarakkeiden kansioiminen
    var cols = table.Columns.ToList();
    foreach(var c in cols)
    {
        var c_n = c.Name;
        string[] parts = c_n.Split('_');
        string groupName = parts[0];
        if (groupName == "Dim")
        {
            c.IsHidden = true;
        }
        else
        {
        Model.Tables[table.Name].Columns[c.Name].DisplayFolder = "Lisävalinnat\\" + groupName;
        }
        
    }
}


// Kansioidaan valittujen taulujen sarakkeet taso* -kansioihin
if(Selected.Tables.Count > 0) 
{
    var selectedTables = Selected.Tables.ToList();
    // Etsii tasoja tällä reseptillä
    string tasoPattern = @"^.*?(_taso\d+)";

    // Käydään läpi valitut taulut
    foreach(var t in selectedTables)
    {
        // Listataan taulun sarakkeet
        var tableCols = t.Columns.ToList();
        string[] tableNameParts = t.Name.Split("_");
        // Käydään läpi sarakkeet
        foreach(var c in tableCols)
        {
            var colName = c.Name;
            Match match = Regex.Match(colName, tasoPattern);
            // Jos sarakenimi sisältää saman osuuden kuin taulunimi, eikä ole avain -> tehdään taso* -kansiot
            if(colName.Contains(tableNameParts[1], StringComparison.OrdinalIgnoreCase) && c.Name.Contains("Dim") == false)
            {
                c.DisplayFolder = "Lisävalinnat\\" + tableNameParts[1] + match.Groups[1].Value;
            }
            // Loput kansioidaan "Muut" -kansioon, avaimia lukuunottamatta
            else if(c.Name.Contains("Dim") == false)
            {
                c.DisplayFolder = "Lisävalinnat\\Muut";
            }
        }
    }
}

/*
    TÄSSÄ TEHDÄÄN HIERARKIA, MUTTA SIIHEN EI PYSTY LISÄTÄ TASOJA, JOTEN TURHA LISÄTÄ
    var t_n = table.Name;
    string[] tableNameParts = t_n.Split('_');
    string hierarchyName = tableNameParts[2];
    // jos moniosainen nimi yhdistetään loppuosat takaisin yhteen:
    if (tableNameParts.Length > 2)
    {
        hierarchyName = string.Join(" ", tableNameParts.Skip(2));
    }
    // ensimäinen kirjain isolla ja hierarkia loppuun
    hierarchyName = char.ToUpper(hierarchyName[0]) + hierarchyName.Substring(1) + " hierarkia";  
    // hierarkia tauluun
    if ( table.Hierarchies.Contains( hierarchyName ))
    {
        Output("Hierarchy already exists");
    }
    else
    {
    var newHierarchy = table.AddHierarchy( hierarchyName );
    }
*/