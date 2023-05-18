using System;
using System.Collections.Generic;
using System.Text;

namespace Morris.Moxy.Providers;

using System;

internal readonly struct ProjectPathAndNamespace : IEquatable<ProjectPathAndNamespace>
{
	public string ProjectPath { get; }
	public string Namespace { get; }

	private readonly Lazy<int> CachedHashCode;

	public ProjectPathAndNamespace(string projectPath, string @namespace)
	{
		ProjectPath = projectPath;
		Namespace = @namespace;
		CachedHashCode = new Lazy<int>(() => (projectPath, @namespace).GetHashCode());
	}

	public static bool operator ==(ProjectPathAndNamespace left, ProjectPathAndNamespace right) => left.Equals(right);
	public static bool operator !=(ProjectPathAndNamespace left, ProjectPathAndNamespace right) => !(left == right);
	public override bool Equals(object obj) => obj is ProjectPathAndNamespace other && Equals(other);

	public bool Equals(ProjectPathAndNamespace other) => 
		ProjectPath == other.ProjectPath
		&& Namespace == other.Namespace;

	public override int GetHashCode() => CachedHashCode.Value;
}

