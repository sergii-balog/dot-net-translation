namespace dotNetWebApiTranslation.Services.Localization;

public interface IResourceLocalizer
{
  string this[string name, string? scope = null] { get; }
}
