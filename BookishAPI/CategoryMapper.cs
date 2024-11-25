namespace BookishAPI;

    public class CategoryMapper
    {
        private static readonly Dictionary<string, string> CategoryMappings = new()
        {
            // Fiction mappings
            { "fiction", "Fiction" },
            { "juvenile fiction", "Fiction" },
            { "young adult fiction", "Fiction" },
            { "literary fiction", "Fiction" },
            { "general fiction", "Fiction" },
            { "mystery & detective", "Mystery" },
            { "mystery", "Mystery" },
            { "thrillers", "Thriller" },
            { "suspense", "Thriller" },
            { "science fiction", "Science Fiction" },
            { "sci-fi", "Science Fiction" },
            { "fantasy", "Fantasy" },
            { "romance", "Romance" },
            { "historical fiction", "Historical Fiction" },
            { "horror", "Horror" },

            // Non-fiction mappings
            { "biography & autobiography", "Biography" },
            { "biography", "Biography" },
            { "autobiography", "Biography" },
            { "memoirs", "Biography" },
            { "business & economics", "Business" },
            { "business", "Business" },
            { "economics", "Business" },
            { "self-help", "Self Help" },
            { "personal growth", "Self Help" },
            
            // Technology mappings
            { "computers", "Technology" },
            { "technology & engineering", "Technology" },
            { "programming", "Technology" },
            { "software", "Technology" },
            
            // Academic mappings
            { "education", "Education" },
            { "teaching", "Education" },
            { "study aids", "Education" },
            { "mathematics", "Mathematics" },
            { "science", "Science" },
            { "medical", "Medical" },
            { "medicine", "Medical" },
            { "psychology", "Psychology" },
            { "philosophy", "Philosophy" },
            { "religion", "Religion" },
            { "spirituality", "Religion" },
            
            // Arts mappings
            { "art", "Art" },
            { "design", "Art" },
            { "music", "Arts & Entertainment" },
            { "performing arts", "Arts & Entertainment" },
            { "photography", "Arts & Entertainment" },
            
            // Lifestyle mappings
            { "cooking", "Cooking & Food" },
            { "food & wine", "Cooking & Food" },
            { "health & fitness", "Health & Fitness" },
            { "exercise", "Health & Fitness" },
            { "travel", "Travel" },
            { "family & relationships", "Lifestyle" },
            { "home & garden", "Lifestyle" },
            { "crafts & hobbies", "Lifestyle" }
        };

        public class CategoryResult
        {
            public string OriginalCategory { get; set; }
            public string NormalizedCategory { get; set; }
            public bool WasMapped { get; set; }
        }

        public CategoryResult MapCategory(string inputCategory)
        {
            if (string.IsNullOrWhiteSpace(inputCategory))
            {
                return new CategoryResult
                {
                    OriginalCategory = inputCategory,
                    NormalizedCategory = "Uncategorized",
                    WasMapped = false
                };
            }

            var normalizedInput = inputCategory.Trim().ToLowerInvariant();

            if (CategoryMappings.TryGetValue(normalizedInput, out string mappedCategory))
            {
                return new CategoryResult
                {
                    OriginalCategory = inputCategory,
                    NormalizedCategory = mappedCategory,
                    WasMapped = true
                };
            }

            // Try to find partial matches
            var partialMatch = CategoryMappings
                .FirstOrDefault(x => normalizedInput.Contains(x.Key) || x.Key.Contains(normalizedInput));

            if (!partialMatch.Equals(default(KeyValuePair<string, string>)))
            {
                return new CategoryResult
                {
                    OriginalCategory = inputCategory,
                    NormalizedCategory = partialMatch.Value,
                    WasMapped = true
                };
            }

            // If no mapping found, return the original category cleaned up
            return new CategoryResult
            {
                OriginalCategory = inputCategory,
                NormalizedCategory = inputCategory.Trim(),
                WasMapped = false
            };
        }

        public IEnumerable<CategoryResult> MapCategories(IEnumerable<string> categories)
        {
            return categories?.Select(MapCategory) ?? 
                   new[] { new CategoryResult { NormalizedCategory = "", WasMapped = false } };
        }
    }

