namespace Morris.Moxy.Models;

using System;

internal readonly struct ProjectInformation : IEquatable<ProjectInformation>
{
	public string Path { get; }
	public string Namespace { get; }

	private readonly Lazy<int> CachedHashCode;

	public ProjectInformation(string path, string @namespace)
	{
		Path = path;
		Namespace = @namespace;
		CachedHashCode = new Lazy<int>(() => HashCode.Combine(path, @namespace));
	}

	public static bool operator ==(ProjectInformation left, ProjectInformation right) => left.Equals(right);
	public static bool operator !=(ProjectInformation left, ProjectInformation right) => !(left == right);
	public override bool Equals(object obj) => obj is ProjectInformation other && Equals(other);

	public bool Equals(ProjectInformation other) =>
		Path == other.Path
		&& Namespace == other.Namespace;

	public override int GetHashCode() => CachedHashCode.Value;
}

