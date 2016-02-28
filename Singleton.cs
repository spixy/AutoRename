namespace AutoRename
{
	public class Singleton<T> where T: new()
	{
		private static T _instance;

		/// <summary>
		/// Get singleton instance
		/// </summary>
		public static T Instance
		{
			get
			{
				if (_instance == null)
					_instance = new T();

				return _instance;
			}
		}
	}
}
