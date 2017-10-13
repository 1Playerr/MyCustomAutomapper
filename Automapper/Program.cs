namespace MyCustomAutomapper
{
	class Program
	{
		static void Main(string[] args)
		{
			var mapGenerator = new MappingGenerator();
			var mapper = mapGenerator.Create<Person, PersonVM>()
				.ForMember(from => from.Age, to => to.NumberOfYears)
				.ForMember(from => from.Name, to => to.FirstName);

			Person person = new Person()
			{
				Age = 22,
				IsStudent = true,
				Name = "Jessika",
				Surname = "White"
			};

			var personVM = mapper.Map(person);
		}
	}
}
