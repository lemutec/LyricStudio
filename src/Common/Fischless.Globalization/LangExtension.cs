using Antelcat.I18N.Abstractions;
using Avalonia;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Metadata;
using Avalonia.Threading;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Fischless.Globalization;

/// <summary>
/// namespace Avalonia.Markup.Xaml.MarkupExtensions;
/// </summary>
[DebuggerDisplay("Key = {Key}, Keys = {Keys}")]
public partial class LangExtension : MarkupExtension, IAddChild
{
    private class ResourceChangedNotifier(ExpandoObject source) : INotifyPropertyChanged
    {
        [CompilerGenerated]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private ExpandoObject _003Csource_003EP = source;

        private ResourceProvider? lastRegister;

        public ExpandoObject Source => _003Csource_003EP;

        public event PropertyChangedEventHandler? PropertyChanged;

        public void ForceUpdate()
        {
            OnPropertyChanged(nameof(Source));
        }

        private void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void RegisterProvider(ResourceProvider provider)
        {
            ResourceProvider provider2 = provider;
            lastRegister = provider2;
            provider2.ChangeCompleted += delegate
            {
                if (lastRegister == provider2)
                {
                    OnPropertyChanged(nameof(Source));
                }
            };
        }
    }

    private delegate void CultureChangedHandler(CultureInfo culture);

    private class MultiValueLangConverter : IMultiValueConverter
    {
        [CompilerGenerated]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly IReadOnlyList<bool> _003CisBindingList_003EP;

        public IValueConverter? Converter { get; set; }

        public object? ConverterParameter { get; set; }

        public MultiValueLangConverter(IReadOnlyList<bool> isBindingList)
        {
            _003CisBindingList_003EP = isBindingList;
        }

        public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
        {
            object source = values[0];
            int count = values.Count;
            object[] array = new object[count - 2];
            string text = (_003CisBindingList_003EP[0] ? GetValue(source, values[1]?.ToString()) : values[1]?.ToString());
            if (string.IsNullOrEmpty(text) || count <= 2)
            {
                return Converter?.Convert(text, targetType, ConverterParameter, culture) ?? text;
            }

            for (int i = 1; i < _003CisBindingList_003EP.Count; i++)
            {
                object obj = values[i + 1];
                if (obj == null)
                {
                    array[i - 1] = string.Empty;
                }
                else
                {
                    array[i - 1] = (_003CisBindingList_003EP[i] ? GetValue(source, obj.ToString()) : obj.ToString());
                }
            }

            string text2 = string.Format(text, array);
            return Converter?.Convert(text2, targetType, ConverterParameter, culture) ?? text2;
        }

        private static string GetValue(object source, string? key)
        {
            return (key == null) ? string.Empty : (((IDictionary<string, object>)source).TryGetValue(key, out object value) ? ((value as string) ?? key) : key);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    private readonly AvaloniaObject proxy = new();

    private static readonly AvaloniaProperty KeyProperty;

    private static readonly AvaloniaProperty TargetPropertyProperty;

    private static readonly IDictionary<string, object?> Target;

    private static readonly ResourceChangedNotifier Notifier;

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    [DefaultValue(null)]
    [Content]
    public Collection<IBinding> Keys { get; } = [];

    public static CultureInfo Culture
    {
        set
        {
            LangExtension.CultureChanged?.Invoke(value);
        }
    }

    [DefaultValue(null)]
    public object? Key
    {
        get => proxy.GetValue(KeyProperty);
        set => proxy.SetValue(KeyProperty, value);
    }

    [DefaultValue(null)]
    public IValueConverter? Converter { get; set; }

    [DefaultValue(null)]
    public object? ConverterParameter { get; set; }

    private static event CultureChangedHandler? CultureChanged;

    static LangExtension()
    {
        KeyProperty = AvaloniaProperty.RegisterAttached<LangExtension, AvaloniaObject, object>("Key");
        TargetPropertyProperty = AvaloniaProperty.RegisterAttached<LangExtension, AvaloniaObject, AvaloniaProperty>("TargetProperty");
        ExpandoObject expandoObject = (ExpandoObject)(Target = new ExpandoObject());
        Notifier = new ResourceChangedNotifier(expandoObject);
        ExpandoObjectPropertyAccessorPlugin.Register(expandoObject);
        List<Action> updateActions = [];
        lock (ResourceProvider.Providers)
        {
            updateActions.AddRange(
                ResourceProvider.Providers.Select(provider => RegisterLanguageSource(provider, true)));

            ResourceProvider.Providers.CollectionChanged += (object? _, NotifyCollectionChangedEventArgs e) =>
            {
                if (e.Action != 0)
                {
                    return;
                }

                foreach (ResourceProvider item2 in e.NewItems.OfType<ResourceProvider>())
                {
                    RegisterLanguageSource(item2, lazyInit: false);
                }
            };
        }

        Dispatcher.UIThread.InvokeAsync(delegate
        {
            foreach (Action item3 in updateActions)
            {
                item3();
            }

            Notifier.ForceUpdate();
        });
    }

    private static void SetTargetProperty(AvaloniaObject element, AvaloniaProperty value)
    {
        element.SetValue(TargetPropertyProperty, value);
    }

    private static AvaloniaProperty GetTargetProperty(AvaloniaObject element)
    {
        return (AvaloniaProperty)element.GetValue(TargetPropertyProperty);
    }

    private object ProvideValueInternal(IServiceProvider serviceProvider)
    {
        if (serviceProvider.GetService(typeof(IProvideValueTarget)) is not IProvideValueTarget provideValueTarget)
        {
            return this;
        }

        StyledElement styledElement = provideValueTarget.TargetObject as StyledElement;
        if (styledElement == null)
        {
            if (provideValueTarget.TargetObject is not LangExtension)
            {
                return this;
            }

            FieldInfo field = provideValueTarget.GetType().GetField("ParentsStack");
            if (field is null)
            {
                return this;
            }

            if (field.GetValue(provideValueTarget) is not IList<object> source)
            {
                return this;
            }

            styledElement = source.Last() as StyledElement;
        }

        if (provideValueTarget.TargetProperty is not AvaloniaProperty targetProperty)
        {
            return this;
        }

        CheckArgument();
        IBinding binding = CreateBinding();
        if (binding is MultiBinding)
        {
            SetTarget(styledElement, targetProperty);
        }

        return binding;
    }

    private void SetTarget(AvaloniaObject targetObject, AvaloniaProperty targetProperty)
    {
        if (targetObject is StyledElement styledElement)
        {
            SetTargetProperty(styledElement, targetProperty);
            styledElement.DataContextChanged += OnDataContextChanged;
        }
    }

    private void OnDataContextChanged(object? sender, EventArgs e)
    {
        if (sender is StyledElement styledElement)
        {
            styledElement.DataContextChanged -= OnDataContextChanged;
            ResetBinding(styledElement);
        }
    }

    private static void SetBinding(AvaloniaObject element, AvaloniaProperty targetProperty, IBinding binding)
    {
        element.Bind(targetProperty, binding);
    }

    private MultiBinding CreateMultiBinding()
    {
        return new MultiBinding
        {
            Mode = BindingMode.OneWay,
            ConverterParameter = ConverterParameter,
            Priority = BindingPriority.LocalValue,
            TargetNullValue = string.Empty
        };
    }

    private IBinding CreateBinding()
    {
        Binding keyBinding = null;
        if (Key is not string text)
        {
            return MapMultiBinding(keyBinding);
        }

        keyBinding = new Binding(text)
        {
            Source = Target,
            Mode = BindingMode.OneWay,
            FallbackValue = text,
            Priority = BindingPriority.LocalValue
        };
        Collection<IBinding> keys = Keys;
        if (keys != null && keys.Count > 0)
        {
            return MapMultiBinding(keyBinding);
        }

        keyBinding.Converter = Converter;
        keyBinding.ConverterParameter = ConverterParameter;
        return keyBinding;
    }

    public static string? Translate(string key, string? fallbackValue = null)
    {
        return Target.TryGetValue(key, out object value) ? ((value as string) ?? fallbackValue) : fallbackValue;
    }

    private static Action RegisterLanguageSource(ResourceProvider provider, bool lazyInit)
    {
        ResourceProvider provider2 = provider;
        CultureChanged += delegate (CultureInfo culture)
        {
            provider2.Culture = culture;
        };
        PropertyInfo[] props = provider2.GetType().GetProperties();
        provider2.PropertyChanged += (object? o, PropertyChangedEventArgs e) =>
        {
            Update(o, e.PropertyName);
        };
        Notifier.RegisterProvider(provider2);
        if (!lazyInit)
        {
            LazyInitAction();
        }

        return LazyInitAction;
        void LazyInitAction()
        {
            PropertyInfo[] array = props;
            foreach (PropertyInfo propertyInfo in array)
            {
                Update(provider2, propertyInfo.Name);
            }
        }

        void Update(object source, string propertyName)
        {
            string propertyName2 = propertyName;
            object obj = Array.Find(props, (PropertyInfo x) => x.Name.Equals(propertyName2))?.GetValue(source, null);
            if (obj != null)
            {
                Target[propertyName2] = obj;
            }
        }
    }

    public LangExtension()
    {
    }

    public LangExtension(string key)
    {
        Key = key;
    }

    public LangExtension(BindingBase binding)
    {
        Key = binding;
    }

    [SuppressMessage("Usage", "CA2208:Instantiate argument exceptions correctly")]
    private MultiBinding MapMultiBinding(Binding? keyBinding)
    {
        MultiBinding multiBinding = CreateMultiBinding();
        Binding item = new("Source")
        {
            Source = Notifier,
            Mode = BindingMode.OneWay
        };
        List<bool> list = [];
        multiBinding.Bindings.Add(item);
        multiBinding.Bindings.Add(keyBinding ?? (Key as BindingBase));
        list.Add(keyBinding == null);
        foreach (IBinding key in Keys)
        {
            IBinding binding = key;
            IBinding binding2 = binding;
            if (binding2 is not LanguageBinding languageBinding)
            {
                if (binding2 is not BindingBase item2)
                {
                    throw new ArgumentException(string.Format("{0} only accept {1} or {2} current type is {3}", "Keys", typeof(LanguageBinding), typeof(Binding), key.GetType()));
                }

                multiBinding.Bindings.Add(item2);
            }
            else
            {
                if (languageBinding.Key == null)
                {
                    throw new ArgumentNullException("Language key should be specified");
                }

                multiBinding.Bindings.Add(new Binding(languageBinding.Key)
                {
                    Source = Target,
                    Mode = BindingMode.OneWay,
                    FallbackValue = languageBinding.Key
                });
            }

            list.Add(key is not LanguageBinding);
        }

        multiBinding.Converter = new MultiValueLangConverter(list.ToArray())
        {
            Converter = Converter,
            ConverterParameter = ConverterParameter
        };
        return multiBinding;
    }

    private void CheckArgument()
    {
        if (Key == null && Keys.Count == 0)
        {
            throw new ArgumentNullException("Key or Keys cannot both be null");
        }

        if (Key == null)
        {
            Collection<IBinding> keys = Keys;
            if (keys != null && keys.Count == 1)
            {
                Key = Keys[0];
                Keys.Clear();
            }
        }
    }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        return MuiLanguage.Mui(Key?.ToString() ?? string.Empty);
        //return ProvideValueInternal(serviceProvider);
    }

    private void ResetBinding(StyledElement element)
    {
        if (Key is not string || !Keys.All((IBinding x) => x is LanguageBinding))
        {
            AvaloniaProperty targetProperty = GetTargetProperty(element);
            SetTargetProperty(element, null);
            IBinding binding = CreateBinding();
            SetBinding(element, targetProperty, binding);
        }
    }

    public void AddChild(object value)
    {
        if (value is Binding item)
        {
            Keys.Add(item);
        }
    }

    public void AddText(string key)
    {
        Keys.Add(new LanguageBinding(key));
    }
}

public sealed class ExpandoObjectPropertyAccessorPlugin(ExpandoObject target) : IPropertyAccessorPlugin
{
    private class ExpandoAccessor : IPropertyAccessor, IDisposable
    {
        private static readonly Dictionary<string, ExpandoAccessor> Accessors = [];

        private static ExpandoObject? source;

        private readonly string propertyName;

        private readonly List<Action<object?>> subscriptions = [];

        public static ExpandoObject? Source
        {
            get => source;
            set
            {
                if (source != null)
                {
                    ((INotifyPropertyChanged)source).PropertyChanged -= OnPropertyChanged;
                }

                if (value != null)
                {
                    source = value;
                    ((INotifyPropertyChanged)source).PropertyChanged += OnPropertyChanged;
                }
            }
        }

        public Type? PropertyType { get; } = typeof(string);

        public object? Value => (Source as IDictionary<string, object>)[propertyName];

        public static ExpandoAccessor Create(string propertyName)
        {
            if (Accessors.TryGetValue(propertyName, out ExpandoAccessor value))
            {
                return value;
            }

            value = new ExpandoAccessor(propertyName);
            Accessors.Add(propertyName, value);
            return value;
        }

        private ExpandoAccessor(string propertyName)
        {
            this.propertyName = propertyName;
        }

        public void Dispose()
        {
            lock (Accessors)
            {
                Accessors.Remove(propertyName);
            }
        }

        private static void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (!Accessors.TryGetValue(e.PropertyName, out ExpandoAccessor value))
            {
                return;
            }

            object obj = (Source as IDictionary<string, object>)[e.PropertyName];
            foreach (Action<object> subscription in value.subscriptions)
            {
                subscription(obj);
            }
        }

        public bool SetValue(object? value, BindingPriority priority)
        {
            throw new NotSupportedException();
        }

        public void Subscribe(Action<object?> listener)
        {
            subscriptions.Add(listener);
        }

        public void Unsubscribe()
        {
            Dispose();
        }
    }

    [CompilerGenerated]
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private ExpandoObject _003Ctarget_003EP = target;

    public bool Match(object obj, string propertyName)
    {
        return obj == _003Ctarget_003EP;
    }

    public IPropertyAccessor Start(WeakReference<object?> reference, string propertyName)
    {
        return ExpandoAccessor.Create(propertyName);
    }

    public static void Register(ExpandoObject target)
    {
        BindingPlugins.PropertyAccessors.Add(new ExpandoObjectPropertyAccessorPlugin(target));
        ExpandoAccessor.Source = target;
    }
}
