using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

// El modelo de datos definido por este archivo sirve como ejemplo representativo de un modelo
// fuertemente tipado que admite notificación cuando se agregan, quitan o modifican miembros. Los nombres
// de propiedad elegidos coinciden con enlaces de datos en las plantillas de elemento estándar.
//
// Las aplicaciones pueden usar este modelo como punto de inicio y ampliarlo o descartarlo completamente
// y reemplazarlo por algo adecuado a sus necesidades.

namespace CelulaW8.Data
{
    /// <summary>
    /// Clase base para <see cref="SampleDataItem"/> y <see cref="SampleDataGroup"/> que
    /// define propiedades comunes a ambos.
    /// </summary>
    [Windows.Foundation.Metadata.WebHostHidden] 
    public abstract class SampleDataCommon : CelulaW8.Common.BindableBase
    {
        private static Uri _baseUri = new Uri("ms-appx:///");

        public SampleDataCommon(String uniqueId, String title, String subtitle, String imagePath, String description)
        {
            this._uniqueId = uniqueId;
            this._title = title;
            this._subtitle = subtitle;
            this._description = description;
            this._imagePath = imagePath;
        }

        private string _uniqueId = string.Empty;
        public string UniqueId
        {
            get { return this._uniqueId; }
            set { this.SetProperty(ref this._uniqueId, value); }
        }

        private string _title = string.Empty;
        public string Title
        {
            get { return this._title; }
            set { this.SetProperty(ref this._title, value); }
        }

        private string _subtitle = string.Empty;
        public string Subtitle
        {
            get { return this._subtitle; }
            set { this.SetProperty(ref this._subtitle, value); }
        }

        private string _description = string.Empty;
        public string Description
        {
            get { return this._description; }
            set { this.SetProperty(ref this._description, value); }
        }

        private ImageSource _image = null;
        private String _imagePath = null;
        public ImageSource Image
        {
            get
            {
                if (this._image == null && this._imagePath != null)
                {
                    this._image = new BitmapImage(new Uri(SampleDataCommon._baseUri, this._imagePath));
                }
                return this._image;
            }

            set
            {
                this._imagePath = null;
                this.SetProperty(ref this._image, value);
            }
        }

        public void SetImage(String path)
        {
            this._image = null;
            this._imagePath = path;
            this.OnPropertyChanged("Image");
        }

        public override string ToString()
        {
            return this.Title;
        }
    }

    /// <summary>
    /// Modelo de datos de elemento genérico.
    /// </summary>
    public class SampleDataItem : SampleDataCommon
    {
        public SampleDataItem(String uniqueId, String title, String subtitle, String imagePath, String description, String content, SampleDataGroup group)
            : base(uniqueId, title, subtitle, imagePath, description)
        {
            this._content = content;
            this._group = group;
        }

        private string _content = string.Empty;
        public string Content
        {
            get { return this._content; }
            set { this.SetProperty(ref this._content, value); }
        }

        private SampleDataGroup _group;
        public SampleDataGroup Group
        {
            get { return this._group; }
            set { this.SetProperty(ref this._group, value); }
        }
    }

    /// <summary>
    /// Modelo de datos de grupo genérico.
    /// </summary>
    public class SampleDataGroup : SampleDataCommon
    {
        public SampleDataGroup(String uniqueId, String title, String subtitle, String imagePath, String description)
            : base(uniqueId, title, subtitle, imagePath, description)
        {
            Items.CollectionChanged += ItemsCollectionChanged;
        }

        private void ItemsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            // Proporciona un subconjunto de la colección completa de elementos al que enlazar desde un elemento GroupedItemsPage
            // por dos motivos: GridView no virtualiza colecciones de elementos grandes y
            // mejora la experiencia del usuario cuando examina grupos con números grandes de
            // elementos.
            //
            // Se muestra un máximo de 12 elementos porque da lugar a columnas de cuadrícula rellenas
            // ya se muestren 1, 2, 3, 4 o 6 filas

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (e.NewStartingIndex < 12)
                    {
                        TopItems.Insert(e.NewStartingIndex,Items[e.NewStartingIndex]);
                        if (TopItems.Count > 12)
                        {
                            TopItems.RemoveAt(12);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Move:
                    if (e.OldStartingIndex < 12 && e.NewStartingIndex < 12)
                    {
                        TopItems.Move(e.OldStartingIndex, e.NewStartingIndex);
                    }
                    else if (e.OldStartingIndex < 12)
                    {
                        TopItems.RemoveAt(e.OldStartingIndex);
                        TopItems.Add(Items[11]);
                    }
                    else if (e.NewStartingIndex < 12)
                    {
                        TopItems.Insert(e.NewStartingIndex, Items[e.NewStartingIndex]);
                        TopItems.RemoveAt(12);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    if (e.OldStartingIndex < 12)
                    {
                        TopItems.RemoveAt(e.OldStartingIndex);
                        if (Items.Count >= 12)
                        {
                            TopItems.Add(Items[11]);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    if (e.OldStartingIndex < 12)
                    {
                        TopItems[e.OldStartingIndex] = Items[e.OldStartingIndex];
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    TopItems.Clear();
                    while (TopItems.Count < Items.Count && TopItems.Count < 12)
                    {
                        TopItems.Add(Items[TopItems.Count]);
                    }
                    break;
            }
        }

        private ObservableCollection<SampleDataItem> _items = new ObservableCollection<SampleDataItem>();
        public ObservableCollection<SampleDataItem> Items
        {
            get { return this._items; }
        }

        private ObservableCollection<SampleDataItem> _topItem = new ObservableCollection<SampleDataItem>();
        public ObservableCollection<SampleDataItem> TopItems
        {
            get {return this._topItem; }
        }
    }

    /// <summary>
    /// Crea una colección de grupos y elementos con contenido codificado de forma rígida.
    /// 
    /// SampleDataSource se inicializa con datos de marcadores de posición en lugar de con datos de producción
    /// activos, por lo que se proporcionan datos de muestra tanto en el momento del diseño como en el de la ejecución.
    /// </summary>
    public sealed class SampleDataSource
    {
        /// <summary>
        /// 
        /// </summary>
        private static SampleDataSource _sampleDataSource = new SampleDataSource();

        /// <summary>
        /// 
        /// </summary>
        private ObservableCollection<SampleDataGroup> _allGroups = new ObservableCollection<SampleDataGroup>();

        /// <summary>
        /// 
        /// </summary>
        public ObservableCollection<SampleDataGroup> AllGroups
        {
            get { return this._allGroups; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uniqueId"></param>
        /// <returns></returns>
        public static IEnumerable<SampleDataGroup> GetGroups(string uniqueId)
        {
            if (!uniqueId.Equals("AllGroups")) throw new ArgumentException("Only 'AllGroups' is supported as a collection of groups");
            
            return _sampleDataSource.AllGroups;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uniqueId"></param>
        /// <returns></returns>
        public static SampleDataGroup GetGroup(string uniqueId)
        {
            // La búsqueda lineal sencilla es aceptable para conjuntos de datos reducidos
            var matches = _sampleDataSource.AllGroups.Where((group) => group.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uniqueId"></param>
        /// <returns></returns>
        public static SampleDataItem GetItem(string uniqueId)
        {
            // La búsqueda lineal sencilla es aceptable para conjuntos de datos reducidos
            var matches = _sampleDataSource.AllGroups.SelectMany(group => group.Items).Where((item) => item.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        public SampleDataSource()
        {
            String ITEM_CONTENT = String.Format("Item Content:{0}",
                        "Item Description:");

            var group1 = new SampleDataGroup("Group-1",
                    "Quienes Somos",
                    "Célula Estudiantil Microsoft ESPOL",
                    "Assets/Logo.png",
                    "Impulsando tu Potencial");
            group1.Items.Add(new SampleDataItem("Group-1-Item-1",
                    "Misión",
                    "Misión Celula Microsoft ESPOL",
                    "Assets/MisionCelula.png",
                    "Impulsando tu Potencial",
                    "Fortalecer el desarrollo académico y profesional de sus miembros, formando a los estudiantes con capacidades de: liderar, investigar y emprender, compartiendo sus conocimientos con la comunidad politécnica y afines, sin descuidar sus valores éticos y morales; promoviendo la creatividad e impulsando proyectos que mejoren la calidad de vida en el ámbito económico, social y ambiental.",
                    group1));
            group1.Items.Add(new SampleDataItem("Group-1-Item-2",
                    "Vision",
                    "Célula Estudiantil Microsoft ESPOL",
                    "Assets/Logo.png",
                    "Impulsando tu Potencial",
                    "Ser referencia dentro de la ESPOL como un club donde sus miembros obtengan un rápido aprendizaje y capacitación constate con conocimientos solidos capaces de contribuir con el desarrollo económico, social y ambiental con tecnologías avanzadas, buscando siempre la excelencia.",
                    group1));            
            this.AllGroups.Add(group1);

            var group2 = new SampleDataGroup("Group-2",
                    "Ramas",
                    "Célula Estudiantil Microsoft ESPOL",
                    "Assets/Logo.png",
                    "Diferentes Ramas de la Célula Microsoft Espol");
            group2.Items.Add(new SampleDataItem("Group-2-Item-1",
                    "Developers",
                    "",
                    "Assets/DV.png",
                    "Esta rama de la Célula Estudiantil Microsoft ESPOL, se dedica a explorar los distintos lenguajes de programación proporcionados por Microsoft o lenguajes Foráneos, para ser utilizados e soluciones informáticas: de escritorio, dispositivos móviles (Smartphone, Tables, etc...) y de web, en distintos niveles de la programación que van desde la implementación hasta la Ingeniería de Software.",
                    "Esta rama de la Célula Estudiantil Microsoft ESPOL, se dedica a explorar los distintos lenguajes de programación proporcionados por Microsoft o lenguajes Foráneos, para ser utilizados e soluciones informáticas: de escritorio, dispositivos móviles (Smartphone, Tables, etc...) y de web, en distintos niveles de la programación que van desde la implementación hasta la Ingeniería de Software.",
                    group2));
            group2.Items.Add(new SampleDataItem("Group-2-Item-2",
                    "Games & Interaction",
                    "",
                    "Assets/GI.png",
                    "Esta rama de la Célula Estudiantil Microsoft ESPOL, busca propulsar la investigación en un entorno de interacción hombre-Máquina, y el aprendizaje a través de sistemas multimedia para lograr así uno de los aspectos más importantes dentro de las soluciones informáticas y de desarrollo de juegos.",
                    "\n\n\nEsta rama de la Célula Estudiantil Microsoft ESPOL, busca propulsar la investigación en un entorno de interacción hombre-Máquina, y el aprendizaje a través de sistemas multimedia para lograr así uno de los aspectos más importantes dentro de las soluciones informáticas y de desarrollo de juegos.",
                    group2));
            group2.Items.Add(new SampleDataItem("Group-2-Item-3",
                    "Robotic",
                    "",
                    "Assets/RB.png",
                    "Esta rama de la Célula Estudiantil Microsoft ESPOL, se encarga de la Robótica que siendo ésta, una de las ramas más importantes de ingeniería de este siglo, no podía quedarse sin su espacio dentro de la tecnologías de Microsoft, ya uue por medio de equipos de robótica y el Software Microsoft Robotics Studios, se puede promover el estudio y la investigación de este campo.",
                    "\n\n\nEsta rama de la Célula Estudiantil Microsoft ESPOL, se encarga de la Robótica que siendo ésta, una de las ramas más importantes de ingeniería de este siglo, no podía quedarse sin su espacio dentro de la tecnologías de Microsoft, ya uue por medio de equipos de robótica y el Software Microsoft Robotics Studios, se puede promover el estudio y la investigación de este campo.",
                    group2));
            group2.Items.Add(new SampleDataItem("Group-2-Item-4",
                    "Busissnes Solution",
                    "",
                    "Assets/BS.png",
                    "Esta rama de la Célula Estudiantil Microsoft ESPOL, trabaja en la economía actual, altamente competitiva tanto para pequeñas, medianas y grandes empresas buscando soluciones de negocios que permitan generar mayor productividad, busca capacitar y dar soluciones de aspecto empresarial a una medida de gran escala.",
                    "\n\n\nEsta rama de la Célula Estudiantil Microsoft ESPOL, trabaja en la economía actual, altamente competitiva tanto para pequeñas, medianas y grandes empresas buscando soluciones de negocios que permitan generar mayor productividad, busca capacitar y dar soluciones de aspecto empresarial a una medida de gran escala.",
                    group2));
            group2.Items.Add(new SampleDataItem("Group-2-Item-5",
                    "Infrastructure Team - Information Tecnologic",
                    "",
                    "Assets/it.png",
                    "Esta rama de la Célula Estudiantil Microsoft ESPOL, se encarga de la investigación e implementación de Tecnologías de Infraestructura y Sistemas de Información que son las partes más fundamentales para el buen desempeño y eficiencia de una empresa, esto incluye lo que es administración de servidores y base de datos, ubicación de los dispositivos de red, seguridad a nivel lógico.",
                    "\n\n\nEsta rama de la Célula Estudiantil Microsoft ESPOL, se encarga de la investigación e implementación de Tecnologías de Infraestructura y Sistemas de Información que son las partes más fundamentales para el buen desempeño y eficiencia de una empresa, esto incluye lo que es administración de servidores y base de datos, ubicación de los dispositivos de red, seguridad a nivel lógico.",
                    group2));
            group2.Items.Add(new SampleDataItem("Group-2-Item-6",
                    "Visual Comunicator",
                    "",
                    "Assets/vc.png",
                    "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
                    "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
                    group2));
            group2.Items.Add(new SampleDataItem("Group-2-Item-7",
                    "Interoperability",
                    "",
                    "Assets/IO.png",
                    "Esta rama de la Célula Estudiantil Microsoft ESPOL, busca crear puentes entra la tecnologías Microsoft y la No-Microsoft, en decir tecnologías de software pagado y software libre, a fin de que se pueda sacar el mayor provecho a la simbiosis de dos tipos de tecnologías.",
                    "\n\n\nEsta rama de la Célula Estudiantil Microsoft ESPOL, busca crear puentes entra la tecnologías Microsoft y la No-Microsoft, en decir tecnologías de software pagado y software libre, a fin de que se pueda sacar el mayor provecho a la simbiosis de dos tipos de tecnologías.",
                    group2));
            this.AllGroups.Add(group2);

            var group3 = new SampleDataGroup("Group-3",
                    "Eventos",
                    "Célula Estudiantil Microsoft ESPOL",
                    "Assets/logo_celula.png",
                    "Impulsando tu Potencial");
            group3.Items.Add(new SampleDataItem("Group-3-Item-1",
                    "bienvenida a novatos 2013-2014 I termino",
                    "",
                    "Assets/logo_celula.png",
                    "contenido",
                    "contenido",
                    group3));
            group3.Items.Add(new SampleDataItem("Group-3-Item-2",
                    "integración con los clubes de ESPOL",
                    "",
                    "Assets/logo_celula.png",
                    "contenido",
                    "contenido",
                    group3));
            group3.Items.Add(new SampleDataItem("Group-3-Item-3",
                    "convenio de cooperación con la UPS",
                    "",
                    "Assets/logo_celula.png",
                    "contenido",
                    "contenido",
                    group3));
            this.AllGroups.Add(group3);

            var group4 = new SampleDataGroup("Group-4",
                "Contactanos",
                "Célula Estudiantil Microsoft ESPOL",
                "Assets/logo_celula.png",
                "Impulsando tu Potencial");
            //group3.Items.Add(new SampleDataItem("Group-4-Item-1",
            //        "Misión",
            //        "Misión Celula Microsoft ESPOL",
            //        "Assets/logo_celula.png",
            //        "Esta rama de la célula, se dedica explorar los distintos lenguajes de programación proporcionados por Microsoft o lenguajes foráneos, para ser utilizados en soluciones informáticas de escritorio, de TabletPC o de Smartphones de distinto nivel, que van desde implementación hasta Ingeniería de Software",
            //        ITEM_CONTENT,
            //        group4));
            this.AllGroups.Add(group4);
        }
    }
}
