using static ProfileGenerator.ProfileGenerator;

namespace ProfileGenerator
{
    public class Profiles : List<Ini> { };

    public static class ProfileGenerator
    {
        public static Profiles LoadProfiles(string directory, Func<string, bool>? processFile = null)
        {
            
            var profiles = new Profiles();
            foreach (var file in Directory.GetFiles(directory))
            {
                if (processFile == null || processFile(file)) { profiles.Add(new Ini(file)); }
            }
            return profiles;
        }


        public enum IniCompareState
        {
            Empty,
            Equal,
            Different,
            UniqueA,
            UniqueB,
        }

        public enum IniCompareResult
        {
            UseA,
            UseB,
            UseNew,
            Skip,
        }

        public struct IniCompare
        {
            public string Section;
            public string key;
            public string valueA;
            public string valueB;
            public string ValueDest;
            public IniCompareState State;
        }

        public delegate IniCompareResult MergeStrategy(IniCompare iniCompare, out string newValue);

    }

    public static class IniExtensions
    {

        public static IniCompareResult DefaultMergeStrategy(IniCompare iniCompare, out string newValue)
        {
            newValue = "";
            switch (iniCompare.State)
            {
                case IniCompareState.Empty:
                    return IniCompareResult.Skip;
                case IniCompareState.Different:
                case IniCompareState.UniqueB:
                    return IniCompareResult.UseB;
                case IniCompareState.Equal:
                case IniCompareState.UniqueA:
                default:
                    return IniCompareResult.UseA;
            }
        }

        

        public static void Merge(this Ini self, Ini iniA, Ini iniB, MergeStrategy? mergeStrategy = null)
        {
            if (mergeStrategy == null) mergeStrategy = DefaultMergeStrategy;

            // Go through all sections in IniA 
            foreach (var section in iniA.GetSections())
            {
                // Go through all keys in the section in IniA. 
                foreach (var key in iniA.GetKeys(section))
                {
                    var valueA = iniA.GetValue(key, section);
                    var valueB = iniB.GetValue(key, section);
                    DoMerge(self, mergeStrategy, section, key, valueA, valueB);
                }

                // Go through all keys in IniB that where not present in the section in IniA. 
                foreach (var key in iniB.GetKeys(section))
                {
                    if (iniA.ContainsKey(section, key)) continue;
                    var valueA = "";
                    var valueB = iniB.GetValue(key, section);
                    DoMerge(self, mergeStrategy, section, key, valueA, valueB);
                }
            }

            // Go through all sections in IniB that where not present in in IniA. 
            foreach (var section in iniB.GetSections())
            {
                if (iniA.ContainsSection(section)) continue;
                foreach (var key in iniB.GetKeys(section))
                {
                    var valueA = "";
                    var valueB = iniB.GetValue(key, section);
                    DoMerge(self, mergeStrategy, section, key, valueA, valueB);
                }
            }
        }

        private static void DoMerge(Ini self, MergeStrategy? mergeStrategy, string section, string key, string valueA, string valueB)
        {
            var comparison = new IniCompare
            {
                Section = section,
                key = key,
                valueA = valueA,
                valueB = valueB,
                State = (valueA == valueB) ?
                          ((valueA == "") ? IniCompareState.Empty : IniCompareState.Equal) :
                          ((valueA == "") ? IniCompareState.UniqueB :
                              ((valueB == "") ? IniCompareState.UniqueA : IniCompareState.Different))
            };
            var result = mergeStrategy(comparison, out string newValue);
            switch (result)
            {
                case IniCompareResult.UseA:
                    self.SetValue(key, section, valueA);
                    break;
                case IniCompareResult.UseB:
                    self.SetValue(key, section, valueB);
                    break;
                case IniCompareResult.UseNew:
                    self.SetValue(key, section, newValue);
                    break;
                case IniCompareResult.Skip:
                    break;
                default:
                    break;
            }
        }
    }
}
