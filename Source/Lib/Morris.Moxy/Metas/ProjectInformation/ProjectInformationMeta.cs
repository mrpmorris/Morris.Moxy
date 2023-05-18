namespace Morris.Moxy.Metas.ProjectInformation;

internal readonly struct ProjectInformationMeta : IEquatable<ProjectInformationMeta>
{
	public string Path { get; }
	public string Namespace { get; }

	private readonly Lazy<int> CachedHashCode;

	public ProjectInformationMeta(string path, string @namespace)
	{
		Path = path;
		Namespace = @namespace;
		CachedHashCode = new Lazy<int>(() => HashCode.Combine(path, @namespace));
	}

	public static bool operator ==(ProjectInformationMeta left, ProjectInformationMeta right) => left.Equals(right);
	public static bool operator !=(ProjectInformationMeta left, ProjectInformationMeta right) => !(left == right);
	public override bool Equals(object obj) => obj is ProjectInformationMeta other && Equals(other);

	public bool Equals(ProjectInformationMeta other) =>
		Path == other.Path
		&& Namespace == other.Namespace;

	public override int GetHashCode() => CachedHashCode.Value;
}

