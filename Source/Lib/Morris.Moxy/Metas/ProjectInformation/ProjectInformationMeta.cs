namespace Morris.Moxy.Metas.ProjectInformation;

internal readonly struct ProjectInformationMeta : IEquatable<ProjectInformationMeta>
{
	public readonly string Path;
	public readonly string Namespace;

	private readonly Lazy<int> CachedHashCode;

	public ProjectInformationMeta()
	{
		Path = "";
		Namespace = "";
		CachedHashCode = new Lazy<int>(() => typeof(ProjectInformationMeta).GetHashCode());
	}

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
		ReferenceEquals(this, other)
		||
		(
			CachedHashCode.IsValueCreated == other.CachedHashCode.IsValueCreated == true
				? CachedHashCode.Value == other.CachedHashCode.Value
				: true
			&& Path == other.Path
			&& Namespace == other.Namespace
		);

	public override int GetHashCode() => CachedHashCode.Value;
}

