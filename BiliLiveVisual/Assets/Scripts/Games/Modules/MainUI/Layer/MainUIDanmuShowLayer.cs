
using THGame.UI;
namespace BLVisual
{
    public class MainUIDanmuShowLayer : FWidget
    {
        FComponent stage;

        protected override void OnInitUI()
        {
            stage = GetChild<FComponent>("stage");
        }

        protected void UpdateLayer()
        {


        }


        protected override void OnInitEvent()
        {

        }

        protected override void OnEnter()
        {
            UpdateLayer();
        }

        protected override void OnExit()
        {

        }
    }
}
