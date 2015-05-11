using CelulaW8.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// La plantilla de elemento Página de detalles de elemento está documentada en http://go.microsoft.com/fwlink/?LinkId=234232

namespace CelulaW8
{
    /// <summary>
    /// Página en la que se muestran los detalles de un único elemento contenido en un grupo y se permite realizar gestos para
    /// pasar a otros elementos pertenecientes al mismo grupo.
    /// </summary>
    public sealed partial class ItemDetailPage : CelulaW8.Common.LayoutAwarePage
    {
        public ItemDetailPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Rellena la página con el contenido pasado durante la navegación. Cualquier estado guardado se
        /// proporciona también al crear de nuevo una página a partir de una sesión anterior.
        /// </summary>
        /// <param name="navigationParameter">Valor de parámetro pasado a
        /// <see cref="Frame.Navigate(Type, Object)"/> cuando se solicitó inicialmente esta página.
        /// </param>
        /// <param name="pageState">Diccionario del estado mantenido por esta página durante una sesión
        /// anterior. Será null la primera vez que se visite una página.</param>
        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            // Permitir que el estado guardado de la página invalide el elemento inicial para mostrarse
            if (pageState != null && pageState.ContainsKey("SelectedItem"))
            {
                navigationParameter = pageState["SelectedItem"];
            }

            // TODO: Crear un modelo de datos adecuado para el dominio del problema para reemplazar los datos de ejemplo
            var item = SampleDataSource.GetItem((String)navigationParameter);
            this.DefaultViewModel["Group"] = item.Group;
            this.DefaultViewModel["Items"] = item.Group.Items;
            this.flipView.SelectedItem = item;
        }

        /// <summary>
        /// Mantiene el estado asociado con esta página en caso de que se suspenda la aplicación o
        /// se descarte la página de la memoria caché de navegación. Los valores deben cumplir los requisitos
        /// de serialización de <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="pageState">Diccionario vacío para rellenar con un estado serializable.</param>
        protected override void SaveState(Dictionary<String, Object> pageState)
        {
            var selectedItem = (SampleDataItem)this.flipView.SelectedItem;
            pageState["SelectedItem"] = selectedItem.UniqueId;
        }

        private void flipView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           // this.Frame.Navigate(typeof(ItemDetailPage));
        }
    }
}
