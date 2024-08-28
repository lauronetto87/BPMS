using System.Collections.Generic;

namespace SatelittiBpms.Models.ViewModel
{

    public class FieldJsonViewModel
    {
        public List<ComponentsJsonViewModel> components { get; set; }
    }

    public class ComponentsJsonViewModel
    {
        public string label { get; set; }
        public string placeholder { get; set; }
        public string tooltip { get; set; }
        public WidgetFieldJson widget { get; set; }
        public string type { get; set; }
        public bool input { get; set; }
        public string key { get; set; }
        public bool tableView { get; set; }
        public string prefix { get; set; }
        public string customClass { get; set; }
        public string suffix { get; set; }
        public bool multiple { get; set; }
        public bool unique { get; set; }
        public bool persistent { get; set; }
        public bool hidden { get; set; }
        public bool clearOnHide { get; set; }
        public string refreshOn { get; set; }
        public string redrawOn { get; set; }
        public bool modalEdit { get; set; }
        public bool dataGridLabel { get; set; }
        public string labelPosition { get; set; }
        public string description { get; set; }
        public string errorLabel { get; set; }
        public bool hideLabel { get; set; }
        public bool disabled { get; set; }
        public bool autofocus { get; set; }
        public bool dbIndex { get; set; }
        public bool calculateServer { get; set; }
        public string tabindex { get; set; }
        public string customDefaultValue { get; set; }
        public string calculateValue { get; set; }
        public string validateOn { get; set; }
        public bool allowCalculateOverride { get; set; }
        public bool encrypted { get; set; }
        public bool showCharCount { get; set; }
        public bool showWordCount { get; set; }
        public bool allowMultipleMasks { get; set; }
        public bool mask { get; set; }
        public bool spellcheck { get; set; }
        public string inputType { get; set; }
        public string inputFormat { get; set; }
        public string inputMask { get; set; }
        public string id { get; set; }

        public ValidateFieldJson validate { get; set; }
        public ConditionalFieldJson conditional { get; set; }
        public OverlayFieldJson overlay { get; set; }

        //public bool protected { get; set; } // Usa palavra reservada
        //"attributes": {}, //obj vazio, não sei o que é
        //"properties": {}, //obj vazio, não sei o que é
        //"defaultValue": null, // Não sei o tipo dessa propriedade
    }

    public class WidgetFieldJson
    {
        public string type { get; set; }
    }

    public class ValidateFieldJson
    {
        public bool required { get; set; }
        public bool customPrivate { get; set; }
        public bool strictDateValidation { get; set; }
        public bool multiple { get; set; }
        public bool unique { get; set; }
        public string custom { get; set; }
        public string minLength { get; set; }
        public string maxLength { get; set; }
        public string pattern { get; set; }
    }

    public class ConditionalFieldJson
    {
        public string eq { get; set; }
        //"show": null, // Nulo, não sei o que é
        //"when": null, // Nulo, não sei o que é

    }

    public class OverlayFieldJson
    {
        public string style { get; set; }
        public string left { get; set; }
        public string top { get; set; }
        public string width { get; set; }
        public string height { get; set; }
    }
}
