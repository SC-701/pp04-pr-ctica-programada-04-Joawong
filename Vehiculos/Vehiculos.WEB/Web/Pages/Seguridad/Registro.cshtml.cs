using Abstracciones.Interfaces.Reglas;
using Abstracciones.Seguridad;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Reglas;

namespace Web.Pages.Seguridad
{
    public class RegistroModel : PageModel
    {
        [BindProperty]
        public Usuario usuario { get; set; } = default!;
        private IConfiguracion _configuracion;

        public RegistroModel(IConfiguracion configuracion)
        {
            _configuracion = configuracion;
        }

        public async Task<IActionResult> OnPost()
        {
            ModelState.Remove("usuario.PasswordHash");
            //Validacion de estado del modelo
            if (!ModelState.IsValid)
            {
                var errores = ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .Select(x => $"{x.Key}: {string.Join(", ", x.Value.Errors.Select(e => e.ErrorMessage))}");
                throw new Exception(string.Join(" | ", errores));
            }
            if (!ModelState.IsValid) return Page();

            var hash = Autenticacion.GenerarHash(usuario.Password);
            usuario.PasswordHash = Autenticacion.ObtenerHash(hash);

            string endpoint = _configuracion.ObtenerMetodo("ApiEndPointsSeguridad", "Registro");
            var cliente = new HttpClient();
            var respuesta = await cliente.PostAsJsonAsync<UsuarioBase>(endpoint, usuario);
            respuesta.EnsureSuccessStatusCode();
            return RedirectToPage("../index");
        }
    }
}
