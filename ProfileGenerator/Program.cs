using Newtonsoft.Json.Linq;
using ProfileGenerator;
using System.Text.Json;
using static ProfileGenerator.ProfileGenerator;

var basePrinterName = "Raise3D_Pro3_Base";
var printerNames    = new string[] { "Raise3D_Pro3Plus_Dual", "Raise3D_Pro3Plus_Single", "Raise3D_Pro3_Dual", "Raise3D_Pro3_Single" };

ConsoleOut.Initialize();
Console.WriteLine("Generating new printer definitions");

CreateNozzles();
InsertGCode();
//CreateVariantFiles();
//CreateQualityFiles();

void CreateNozzles()
{
    var nozzleSizes = new string[]    { "0.3", "0.4", "0.5", "0.6", "0.8", "1.0" };
    var iniConfig   = new IniConfig() { PadLeftOfEqual = 1, PadRightOfEqual = 1 };
    

    // For each nozzle size
    foreach (var nozzleSize in nozzleSizes)
    {
        var baseNozzleDefinition = new Ini($"../../../../Cura/Current/Release/variants/{basePrinterName}_{nozzleSize}.inst.cfg", iniConfig);
        // For each different printer 
        foreach (var printerName in printerNames)
        {
            var nozzleDefinition = baseNozzleDefinition.Clone();
            nozzleDefinition.SetValue("definition", "general", printerName);
            nozzleDefinition.Save($"../../../../Cura/Current/Release/variants/{printerName}_{nozzleSize}.inst.cfg");
        }
    }
}



void InsertGCode()
{
    var iniConfig                = new IniConfig() { PadLeftOfEqual = 1       , PadRightOfEqual = 1   };
    var singleNozzlePrinterNames = new string[]    { "Raise3D_Pro3Plus_Single", "Raise3D_Pro3_Single" };
    var doubleNozzleprinterNames = new string[]    { "Raise3D_Pro3Plus_Dual"  , "Raise3D_Pro3_Dual"   };

    var doubleStartGcode = JsonEncodedText.Encode(File.ReadAllText($"../../../../Resources/Gcode/Raise3D_Pro3_Double_Start_Gcode.txt"));
    var doubleStopGcode  = JsonEncodedText.Encode(File.ReadAllText($"../../../../Resources/Gcode/Raise3D_Pro3_Double_Stop_Gcode.txt" ));
    var singleStartGcode = JsonEncodedText.Encode(File.ReadAllText($"../../../../Resources/Gcode/Raise3D_Pro3_Single_Start_Gcode.txt"));
    var singleStopGcode  = JsonEncodedText.Encode(File.ReadAllText($"../../../../Resources/Gcode/Raise3D_Pro3_Single_Stop_Gcode.txt" ));

    // For each nozzle size
    
    foreach (var printerName in singleNozzlePrinterNames)
    {
        var singleNozzlePrinterDefinition = JObject.Parse(File.ReadAllText($"../../../../Cura/Current/Release/definitions/{printerName}.def.json"));
        singleNozzlePrinterDefinition = (JObject)singleNozzlePrinterDefinition.ReplacePath(@"$.overrides.machine_start_gcode.default_value", singleStartGcode);
        singleNozzlePrinterDefinition = (JObject)singleNozzlePrinterDefinition.ReplacePath(@"$.overrides.machine_end_gcode.default_value", singleStopGcode);
        File.WriteAllText($"../../../../Cura/Current/Release/definitions/{printerName}.def.json.temp", singleNozzlePrinterDefinition.ToString());
    }

    foreach (var printerName in doubleNozzleprinterNames)
    {
        var singleNozzlePrinterDefinition = JObject.Parse(File.ReadAllText($"../../../../Cura/Current/Release/definitions/{printerName}.def.json"));
        singleNozzlePrinterDefinition = (JObject)singleNozzlePrinterDefinition.ReplacePath(@"$.overrides.machine_start_gcode.default_value", doubleStartGcode);
        singleNozzlePrinterDefinition = (JObject)singleNozzlePrinterDefinition.ReplacePath(@"$.overrides.machine_end_gcode.default_value", doubleStopGcode);
        File.WriteAllText($"../../../../Cura/Current/Release/definitions/{printerName}.def.json.temp", singleNozzlePrinterDefinition.ToString());
    }
}


void CreateVariantFiles()
{
    var baseProfileDirectory = @"C:\Program Files\Ultimaker Cura 5.2.1\share\cura\resources\variants";
    var destProfileDirectory = @"E:\Github\CR10s-DDE-Orbiter\Cura\variants";
    var variantDdeOrbiterProfilePath = @"E:\Github\CR10s-DDE-Orbiter\Resources\BaseProfiles\creality_dde_orbiter_variant.inst.cfg";


    var iniConfig = new IniConfig() { PadLeftOfEqual = 1, PadRightOfEqual = 1 };


    // **** Create new variant profiles for nozzles based on dde orbiter (updated name)

    // Load base file updates
    var variantDdeOrbiterProfile = new Ini(variantDdeOrbiterProfilePath);

    // Use only base profiles for the merge
    var baseGlobalProfiles = LoadProfiles(baseProfileDirectory, (fileName) => { return fileName.Contains("creality_cr10_"); });

    // Now loop through all base profiles and merge them with the base globale update 
    foreach (var baseGlobalProfile in baseGlobalProfiles)
    {
        var newFileName = Path.GetFileName(baseGlobalProfile.FileName);
        newFileName = newFileName.Replace("creality_cr10_", "creality_cr10_dde_orbiter_");

        var destinationfileName = destProfileDirectory + @"\" + newFileName ;
        var destinationProfile = new Ini(destinationfileName, iniConfig);

        // Merge the profiles. The strategy is important: It determines which settings are used from the base & fast profiles
        destinationProfile.Merge(baseGlobalProfile, variantDdeOrbiterProfile, UpdateWithDdeOrbiterSettingStrategy);
        destinationProfile.Save();
    }
}

void CreateQualityFiles()
{
    
    var baseProfileDirectory            = @"C:\Program Files\Ultimaker Cura 5.2.1\share\cura\resources\quality\creality\base";    
    var baseDdeOrbiterProfilePath       = @"E:\Github\CR10s-DDE-Orbiter\Resources\BaseProfiles\creality_dde_orbiter_base.inst.cfg";
    var baseGlobalDdeOrbiterProfilePath = @"E:\Github\CR10s-DDE-Orbiter\Resources\BaseProfiles\creality_dde_orbiter_base_global.inst.cfg";
    var destProfileDirectory            = @"E:\Github\CR10s-DDE-Orbiter\Cura\quality\creality_cr10s_dde_orbiter";


    var iniConfig = new IniConfig() { PadLeftOfEqual = 1, PadRightOfEqual = 1 };


    // **** Create new global base quality base profiles based on dde orbiter (updated name, retraction)

    // Load base file updates
    var baseGlobalDdeOrbiterProfile = new Ini(baseGlobalDdeOrbiterProfilePath);

    // Use only base profiles for the merge
    var baseGlobalProfiles = LoadProfiles(baseProfileDirectory, (fileName) => { return fileName.Contains("global"); });    

    // Now loop through all base profiles and merge them with the base globale update 
    foreach (var baseGlobalProfile in baseGlobalProfiles)
    {
        var destinationfileName = destProfileDirectory + @"\" + Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(baseGlobalProfile.FileName)) + "_dde_orbiter.inst.cfg";
        var destinationProfile = new Ini(destinationfileName, iniConfig);

        // Merge the profiles. The strategy is important: It determines which settings are used from the base & fast profiles
        destinationProfile.Merge(baseGlobalProfile, baseGlobalDdeOrbiterProfile, UpdateWithDdeOrbiterSettingStrategy);
        destinationProfile.Save();
    }

    // **** Create new non-global base quality base profiles based on dde orbiter (updated name)

    // Load base file updates
    var baseDdeOrbiterProfile = new Ini(baseDdeOrbiterProfilePath);

    // Use only base profiles for the merge
    var baseProfiles = LoadProfiles(baseProfileDirectory, (fileName) => { return !fileName.Contains("global"); });

    // Now loop through all base profiles and merge them with the base globale update 
    foreach (var baseProfile in baseProfiles)
    {
        var destinationfileName = destProfileDirectory + @"\" + Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(baseProfile.FileName)) + "_dde_orbiter.inst.cfg";
        var destinationProfile = new Ini(destinationfileName, iniConfig);

        // Merge the profiles. The strategy is important: It determines which settings are used from the base & fast profiles
        destinationProfile.Merge(baseProfile, baseDdeOrbiterProfile, UpdateWithDdeOrbiterSettingStrategy);
        destinationProfile.Save();
    }


    //var iniConfig = new IniConfig() { PadLeftOfEqual = 1, PadRightOfEqual = 1 }; 

    ////var baseProfiles = LoadProfiles(baseProfileDirectory,(fileName)=> { return !fileName.Contains("global") && fileName.Contains("PLA") && (fileName.Contains("0.4") || fileName.Contains("0.6")); });
    ////var baseProfiles = LoadProfiles(baseProfileDirectory, (fileName) => { return fileName.Contains("global"); });

    //// Use only non-global base profiles for the merge. That is, with material & nozzle size
    //var baseProfiles = LoadProfiles(baseProfileDirectory, (fileName) => { return !fileName.Contains("global"); });

    //// Load all fast profiles
    //var fastProfiles = LoadProfiles(fastProfileDirectory);

    //// Now loop through all base profiles and merge them with the fast profiles
    //foreach (var baseProfile in baseProfiles)
    //{
    //    // Match profiles based on variant (containing nozzle size
    //    var baseProfileVariant = baseProfile.GetValue("variant", "metadata");
    //    var fastProfile        = fastProfiles.Find(profile => profile.GetValue("variant","metadata") == baseProfileVariant) ?? fastProfiles[0];


    //    var destinationfileName = destProfileDirectory + @"\" + Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(baseProfile.FileName)) + "_fast.inst.cfg";
    //    var destinationProfile = new Ini(destinationfileName,iniConfig);

    //    // Merge the profiles. The strategy is important: It determines which settings are used from the base & fast profiles
    //    destinationProfile.Merge(baseProfile, fastProfile, UpdateWithFastSettingStrategy);        
    //    destinationProfile.Save();
    // }
}

IniCompareResult UpdateWithDdeOrbiterSettingStrategy(IniCompare iniCompare, out string newValue)
{
    // Start with default merge strategy, we are going to update this below
    var result = IniExtensions.DefaultMergeStrategy(iniCompare, out newValue);

    // Always use the fast settings if fast settings are different from base profile
    if (iniCompare.State == IniCompareState.Different)
    {
        result = IniCompareResult.UseB;
        // Unless it is wall thickness, then we use base        
    }

    return result;
}

IniCompareResult UpdateWithFastSettingStrategy(IniCompare iniCompare, out string newValue)
{
    // Start with default merge strategy, we are going to update this below
    var result = IniExtensions.DefaultMergeStrategy(iniCompare, out newValue);

    // Always use the fast settings if fast settings are different from base profile
    if (iniCompare.State == IniCompareState.Different)
    {
        result = IniCompareResult.UseB;
        // Unless it is wall thickness, then we use base
        if (iniCompare.key == "wall_thickness") result = IniCompareResult.UseA;         
    }

    // It seems to work best to stay as close to the metadata settings of the base profiles as possible
    if (iniCompare.Section == "metadata")
    {
        result = IniCompareResult.UseA;

        // However, somehow the "quality" type causes the profiles to not be read. Therefore we add them as quality_changes
        if (iniCompare.key == "type")
        {
            newValue = "quality_changes";
            result = IniCompareResult.UseNew;
        }
    }    

    // We also need to update the profile name: If the base setting contains the name, update it with "Fast" postfix.
    if (iniCompare.key == "name")
    {
        newValue = iniCompare.valueA.Trim() + " Fast";
        result = IniCompareResult.UseNew;
    }

    return result;
}


public static class JsonExtensions
{
    public static JToken ReplacePath<T>(this JToken root, string path, T newValue)
    {
        if (root == null || path == null)
            throw new ArgumentNullException();

        foreach (var value in root.SelectTokens(path).ToList())
        {
            if (value == root)
                root = JToken.FromObject(newValue);
            else
                value.Replace(JToken.FromObject(newValue));
        }

        return root;
    }

    public static string ReplacePath<T>(string jsonString, string path, T newValue)
    {
        return JToken.Parse(jsonString).ReplacePath(path, newValue).ToString();
    }
}