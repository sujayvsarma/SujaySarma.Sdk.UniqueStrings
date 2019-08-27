using HashidsNet;

using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SujaySarma.Sdk.Core
{
    /// <summary>
    /// Generate unique strings from a variety of starting points
    /// </summary>
    public static class UniqueStrings
    {

        /// <summary>
        /// Generate the unique string
        /// </summary>
        /// <returns>Unique string of 8 characters. String will be mixed-case!</returns>
        public static string Generate()
        {
            string identifier = "";
            lock (_generatorLock)
            {
                identifier = _generator.Encode(_state.NextNumber);
                _state.NextNumber++;
            }

            try
            {
                File.WriteAllText(_stateFileName, JsonSerializer.Serialize(_state));
            }
            catch
            {
                // we couldnt write the file, its alright.
            }

            return identifier;
        }


        static UniqueStrings()
        {
            _stateFileName = Path.Combine(Directory.GetCurrentDirectory(), "uniques.json");

            try
            {
                if (File.Exists(_stateFileName))
                {
                    _state = JsonSerializer.Deserialize<UniqueState>(File.ReadAllText(_stateFileName));
                }
            }
            catch
            {
                // we couldnt read the file, its alright, use defaults;
                _state = new UniqueState();
            }

            _generator = new Hashids(_state.Salt, 8);
        }

        private static readonly object _generatorLock = new object();
        private static readonly Hashids _generator = null;
        private static readonly UniqueState _state = null;
        private static readonly string _stateFileName = null;

        private class UniqueState
        {

            [JsonPropertyName("salt")]
            public string Salt { get; set; } = Guid.NewGuid().ToString("n");


            [JsonPropertyName("next")]
            public int NextNumber { get; set; } = 1;

        }
    }
}
