namespace GameOfTournamentsTests
{
    using KellermanSoftware.CompareNetObjects;

    public class GameOfTournamentsBaseTests
    {
        protected static bool DeepEqual(object a, object b)
        {
            var compareLogic = new CompareLogic();
            var result = compareLogic.Compare(a, b);
            return result.AreEqual;
        }
    }
}