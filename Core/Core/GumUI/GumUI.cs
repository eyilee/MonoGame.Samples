using Gum.Wireframe;
using MonoGameGum;
using MonoGameGum.GueDeriving;

namespace MonoGame.Samples.Library.GumUI
{
    public abstract class GumUI
    {
        public GraphicalUiElement RootObject => rootObject;

        protected readonly GraphicalUiElement rootObject;

        private bool _instantiated;

        public GumUI ()
        {
            rootObject = new ContainerRuntime ();
            rootObject.Dock (Dock.SizeToChildren);
        }

        public GumUI (GraphicalUiElement rootObject)
        {
            this.rootObject = rootObject;
        }

        protected virtual void OnInstantiate () { }

        private GumUI Instantiate (GraphicalUiElement? parent = null)
        {
            if (_instantiated)
            {
                return this;
            }

            OnInstantiate ();

            if (parent == null)
            {
                rootObject.AddToRoot ();
            }
            else
            {
                parent.AddChild (rootObject);
            }

            _instantiated = true;

            return this;
        }

        protected virtual void OnDetach () { }

        public void Detach ()
        {
            if (!_instantiated)
            {
                return;
            }

            OnDetach ();

            rootObject.RemoveFromRoot ();
        }

        public static T Instantiate<T> (T gumUI, GraphicalUiElement? parent = null) where T : GumUI
        {
            return (T)gumUI.Instantiate (parent);
        }
    }
}
