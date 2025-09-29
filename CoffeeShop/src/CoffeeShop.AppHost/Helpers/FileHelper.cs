namespace Zzaia.CoffeeShop.AppHost.Helpers;

/// <summary>
/// Provides file and directory manipulation utilities for configuration processing
/// </summary>
public static class FileHelper
{
    /// <summary>
    /// Replaces variables in YAML files with provided values
    /// </summary>
    /// <param name="resourcesPaths">Array of resource paths to process</param>
    /// <param name="replacements">Dictionary of variable replacements</param>
    public static void ReplaceVariablesInYamlFiles(string[] resourcesPaths, Dictionary<string, string> replacements)
    {
        foreach (string resourcesPath in resourcesPaths)
        {
            string fullPath = CombineCrossPlatformPath(AppContext.BaseDirectory, resourcesPath);
            if (!Directory.Exists(fullPath))
                continue;

            string[] yamlFiles = Directory.GetFiles(fullPath, "*.yaml", SearchOption.AllDirectories)
                .Concat(Directory.GetFiles(fullPath, "*.yml", SearchOption.AllDirectories))
                .ToArray();

            foreach (string yamlFile in yamlFiles)
            {
                string content = File.ReadAllText(yamlFile);
                foreach (KeyValuePair<string, string> replacement in replacements)
                {
                    content = content.Replace(replacement.Key, replacement.Value);
                }
                File.WriteAllText(yamlFile, content);
            }
        }
    }

    /// <summary>
    /// Combines file paths in a cross-platform manner
    /// </summary>
    /// <param name="basePath">Base directory path</param>
    /// <param name="relativePath">Relative path to combine</param>
    /// <returns>Combined cross-platform path</returns>
    public static string CombineCrossPlatformPath(string basePath, string relativePath)
    {
        string normalizedRelativePath = relativePath.Replace('\\', Path.DirectorySeparatorChar);
        return Path.Combine(basePath, normalizedRelativePath);
    }
}