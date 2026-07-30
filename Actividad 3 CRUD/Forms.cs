using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Actividad3CRUD
{
    internal class BaseForm : Form
    {
        protected BaseForm(string title)
        {
            Text = title + " - Actividad 3: CRUD";
            StartPosition = FormStartPosition.CenterScreen;
            Font = new Font("Segoe UI", 10F);
            BackColor = Color.White;
            MinimumSize = new Size(640, 420);
        }

        protected Button Button(string text, int x, int y, EventHandler click)
        {
            var button = new Button { Text = text, Left = x, Top = y, Width = 112, Height = 34, UseVisualStyleBackColor = true };
            button.Click += click;
            Controls.Add(button);
            return button;
        }

        protected Label Label(string text, int x, int y, int width = 160)
        {
            var label = new Label { Text = text, Left = x, Top = y, Width = width, Height = 25 };
            Controls.Add(label);
            return label;
        }
    }

    internal sealed class MainForm : BaseForm
    {
        private const string ClaveRegistro = "UMI2026";

        internal MainForm() : base("Sistema de compraventa")
        {
            Width = 760; Height = 450;
            var menu = new MenuStrip();
            var bienvenida = new ToolStripMenuItem("Bienvenida");
            bienvenida.DropDownItems.Add("Mensaje de bienvenida", null, (s, e) => ShowInfo("Mensaje de bienvenida", "¡Bienvenido al sistema de compraventa de la zapatería UMI!\r\n\r\nDesde este menú podrás consultar información institucional y acceder a las opciones del sistema."));
            bienvenida.DropDownItems.Add("¿Quiénes somos?", null, (s, e) => ShowInfo("¿Quiénes somos?", "Somos una zapatería orientada a brindar calzado de calidad para damas y caballeros, con atención cercana y productos confiables."));
            bienvenida.DropDownItems.Add("Misión", null, (s, e) => ShowInfo("Misión", "Ofrecer calzado cómodo y de calidad, con un servicio responsable que satisfaga las necesidades de nuestros clientes."));
            bienvenida.DropDownItems.Add("Visión", null, (s, e) => ShowInfo("Visión", "Ser una zapatería reconocida por su variedad, calidad, atención y mejora continua."));

            var productos = new ToolStripMenuItem("Productos");
            productos.DropDownItems.Add("Damas", null, (s, e) => new CatalogForm("Catálogo de damas", new[] { "Zapatilla clásica", "Sandalia casual", "Tenis urbano", "Bota corta" }).ShowDialog(this));
            productos.DropDownItems.Add("Caballeros", null, (s, e) => new CatalogForm("Catálogo de caballeros", new[] { "Zapato formal", "Tenis deportivo", "Bota de piel", "Mocasín" }).ShowDialog(this));

            var registro = new ToolStripMenuItem("Registro");
            registro.DropDownItems.Add("Cliente", null, (s, e) => OpenProtected(() => new ClienteForm()));
            registro.DropDownItems.Add("Proveedor", null, (s, e) => OpenProtected(() => new ProveedorForm()));
            registro.DropDownItems.Add("Producto", null, (s, e) => OpenProtected(() => new ProductoForm()));

            var compras = new ToolStripMenuItem("Compras");
            compras.Click += (s, e) => new CompraForm().ShowDialog(this);
            var salir = new ToolStripMenuItem("Salir");
            salir.Click += (s, e) => Close();
            menu.Items.AddRange(new ToolStripItem[] { bienvenida, productos, registro, compras, salir });
            MainMenuStrip = menu; Controls.Add(menu);
            var title = new Label { Text = "Sistema de compraventa - Zapatería UMI", Font = new Font("Segoe UI", 18F, FontStyle.Bold), AutoSize = true, Left = 110, Top = 120 };
            var help = new Label { Text = "Seleccione una opción del menú para comenzar.", AutoSize = true, Left = 210, Top = 175 };
            Controls.Add(title); Controls.Add(help);
        }

        private void OpenProtected(Func<Form> makeForm)
        {
            using (var password = new PasswordForm())
                if (password.ShowDialog(this) == DialogResult.OK && password.Password == ClaveRegistro)
                    using (var form = makeForm()) form.ShowDialog(this);
                else if (password.DialogResult == DialogResult.OK)
                    MessageBox.Show("Contraseña incorrecta.", "Acceso denegado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void ShowInfo(string title, string text)
        {
            using (var form = new InfoForm(title, text)) form.ShowDialog(this);
        }
    }

    internal sealed class InfoForm : BaseForm
    {
        internal InfoForm(string title, string text) : base(title)
        {
            Width = 620; Height = 330;
            var box = new TextBox { Multiline = true, ReadOnly = true, Text = text, Left = 35, Top = 55, Width = 530, Height = 150, ScrollBars = ScrollBars.Vertical };
            Controls.Add(box); Button("Regresar", 450, 225, (s, e) => Close());
        }
    }

    internal sealed class PasswordForm : BaseForm
    {
        private readonly TextBox input;
        internal string Password { get { return input.Text; } }
        internal PasswordForm() : base("Acceso a Registro")
        {
            Width = 430; Height = 220;
            Label("Contraseña de acceso:", 35, 55, 180);
            input = new TextBox { Left = 205, Top = 52, Width = 160, UseSystemPasswordChar = true };
            Controls.Add(input); AcceptButton = Button("Ingresar", 160, 110, (s, e) => { DialogResult = DialogResult.OK; Close(); });
            Button("Cancelar", 280, 110, (s, e) => Close());
        }
    }

    internal sealed class CatalogForm : BaseForm
    {
        internal CatalogForm(string title, IEnumerable<string> catalog) : base(title)
        {
            Width = 650; Height = 450;
            Label("Tallas disponibles (seleccione las deseadas):", 30, 45, 350);
            var sizes = new CheckedListBox { Left = 30, Top = 75, Width = 180, Height = 240, CheckOnClick = true };
            for (double size = 21; size <= 27; size += .5) sizes.Items.Add(size.ToString("0.0"));
            Controls.Add(sizes);
            Label("Catálogo de productos:", 285, 45, 220);
            var list = new ListBox { Left = 285, Top = 75, Width = 280, Height = 240 };
            list.Items.AddRange(catalog.Cast<object>().ToArray()); Controls.Add(list);
            Button("Regresar", 453, 335, (s, e) => Close());
        }
    }

    internal sealed class FieldSpec
    {
        internal string Name; internal string Caption; internal Type Type;
        internal FieldSpec(string name, string caption, Type type) { Name = name; Caption = caption; Type = type; }
    }

    internal abstract class CrudForm : BaseForm
    {
        private readonly string table, idColumn;
        private readonly FieldSpec[] fields;
        private readonly TextBox idBox = new TextBox();
        private readonly Dictionary<string, Control> inputs = new Dictionary<string, Control>();
        private readonly DataGridView grid = new DataGridView();

        protected CrudForm(string title, string tableName, string idName, params FieldSpec[] specs) : base(title)
        {
            table = tableName; idColumn = idName; fields = specs;
            Width = 1040; Height = 650;
            Label(idName + " (para modificar/eliminar):", 25, 45, 220);
            idBox.Left = 245; idBox.Top = 42; idBox.Width = 150; Controls.Add(idBox);
            int y = 82;
            foreach (var field in fields)
            {
                Label(field.Caption + ":", 25, y, 210);
                Control input = field.Type == typeof(DateTime) ? (Control)new DateTimePicker { Format = DateTimePickerFormat.Short } : new TextBox();
                input.Left = 245; input.Top = y - 3; input.Width = 220; input.Height = 27;
                Controls.Add(input); inputs.Add(field.Name, input); y += 38;
            }
            Button("Agregar", 25, y + 5, (s, e) => Insert());
            Button("Modificar", 145, y + 5, (s, e) => UpdateRecord());
            Button("Eliminar", 265, y + 5, (s, e) => Delete());
            Button("Mostrar", 385, y + 5, (s, e) => LoadData());
            Button("Regresar", 505, y + 5, (s, e) => Close());
            grid.Left = 500; grid.Top = 42; grid.Width = 500; grid.Height = 480; grid.ReadOnly = true; grid.AllowUserToAddRows = false;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect; grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.CellClick += GridCellClick; Controls.Add(grid);
        }

        private SqlParameter[] Parameters(bool includeId)
        {
            var parameters = new List<SqlParameter>();
            if (includeId) parameters.Add(new SqlParameter("@id", Convert.ToInt32(idBox.Text)));
            foreach (var field in fields)
            {
                object value;
                if (field.Type == typeof(DateTime)) value = ((DateTimePicker)inputs[field.Name]).Value.Date;
                else if (field.Type == typeof(decimal)) value = Convert.ToDecimal(((TextBox)inputs[field.Name]).Text);
                else if (field.Type == typeof(int)) value = Convert.ToInt32(((TextBox)inputs[field.Name]).Text);
                else value = ((TextBox)inputs[field.Name]).Text.Trim();
                parameters.Add(new SqlParameter("@" + field.Name, value));
            }
            return parameters.ToArray();
        }

        private void Insert()
        {
            try
            {
                string names = string.Join(", ", fields.Select(f => f.Name));
                string values = string.Join(", ", fields.Select(f => "@" + f.Name));
                Database.Execute("INSERT INTO dbo." + table + " (" + names + ") VALUES (" + values + ")", Parameters(false));
                MessageBox.Show("El registro fue agregado exitosamente.", "Éxito"); LoadData();
            }
            catch (Exception ex) { ShowError(ex); }
        }

        private void UpdateRecord()
        {
            try
            {
                string sets = string.Join(", ", fields.Select(f => f.Name + " = @" + f.Name));
                Database.Execute("UPDATE dbo." + table + " SET " + sets + " WHERE " + idColumn + " = @id", Parameters(true));
                MessageBox.Show("El registro fue modificado exitosamente.", "Éxito"); LoadData();
            }
            catch (Exception ex) { ShowError(ex); }
        }

        private void Delete()
        {
            try
            {
                Database.Execute("DELETE FROM dbo." + table + " WHERE " + idColumn + " = @id", new SqlParameter("@id", Convert.ToInt32(idBox.Text)));
                MessageBox.Show("El registro fue eliminado exitosamente.", "Éxito"); LoadData();
            }
            catch (Exception ex) { ShowError(ex); }
        }

        private void LoadData()
        {
            try { grid.DataSource = Database.Query("SELECT * FROM dbo." + table + " ORDER BY " + idColumn); }
            catch (Exception ex) { ShowError(ex); }
        }

        private void GridCellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var row = grid.Rows[e.RowIndex]; idBox.Text = row.Cells[idColumn].Value.ToString();
            foreach (var field in fields)
            {
                object value = row.Cells[field.Name].Value;
                if (field.Type == typeof(DateTime)) ((DateTimePicker)inputs[field.Name]).Value = Convert.ToDateTime(value);
                else ((TextBox)inputs[field.Name]).Text = Convert.ToString(value);
            }
        }

        private void ShowError(Exception ex)
        {
            MessageBox.Show("No se pudo completar la operación. Verifique que ejecutó Database\\CrearBaseDatos.sql en LocalDB.\r\n\r\nDetalle: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    internal sealed class ClienteForm : CrudForm
    {
        internal ClienteForm() : base("Registro de clientes", "Clientes", "ClienteId",
            new FieldSpec("Dni", "DNI", typeof(string)), new FieldSpec("Nombre", "Nombre", typeof(string)),
            new FieldSpec("Apellidos", "Apellidos", typeof(string)), new FieldSpec("FechaNacimiento", "Fecha de nacimiento", typeof(DateTime)),
            new FieldSpec("Telefono", "Teléfono", typeof(string))) { }
    }
    internal sealed class ProveedorForm : CrudForm
    {
        internal ProveedorForm() : base("Registro de proveedores", "Proveedores", "ProveedorId",
            new FieldSpec("Nif", "NIF", typeof(string)), new FieldSpec("Nombre", "Nombre", typeof(string)),
            new FieldSpec("Direccion", "Dirección", typeof(string))) { }
    }
    internal sealed class ProductoForm : CrudForm
    {
        internal ProductoForm() : base("Registro de productos", "Productos", "ProductoId",
            new FieldSpec("Codigo", "Código", typeof(string)), new FieldSpec("Nombre", "Nombre", typeof(string)),
            new FieldSpec("Precio", "Precio", typeof(decimal)), new FieldSpec("ProveedorId", "ID proveedor", typeof(int))) { }
    }
    internal sealed class CompraForm : CrudForm
    {
        internal CompraForm() : base("Registro de compras", "Compras", "CompraId",
            new FieldSpec("ClienteId", "ID cliente", typeof(int)), new FieldSpec("ProductoId", "ID producto", typeof(int))) { }
    }
}
