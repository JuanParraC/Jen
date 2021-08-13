namespace Jen
{
	using System;
	using System.Collections;
	public interface ILectorBD  : IDisposable
    {
        bool HasRows { get; }
        bool IsScalar { get; }
        int FieldCount { get; }
        bool Read();
        void Close();
        string GetName(int i);
        string GetString(int i);
        object GetValue(int i);
        object GetScalar();
		IEnumerator GetEnumerator();
    }
}
