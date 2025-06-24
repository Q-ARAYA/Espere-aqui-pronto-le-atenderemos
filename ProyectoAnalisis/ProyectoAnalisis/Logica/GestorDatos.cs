// Archivo: Logica/GestorDatos.cs
using System.Collections.Generic;
using System.Linq; // Necesario para .Any()

namespace ProyectoAnalisis.Logica
{
    public static class GestorDatos
    {
        // Lista "maestra" de todas las especialidades en el sistema
        // Es privada para que solo se pueda modificar a traves de los métodos de esta clase
        private static List<Especialidades> listaDeEspecialidades = new List<Especialidades>();

        /// <summary>
        /// Agrega una nueva especialidad a la lista maestra.
        /// </summary>
        /// <param name="especialidad">La especialidad a agregar.</param>
        /// <returns>True si se agregó, False si ya existía una con el mismo nombre.</returns>
        // Sirve para agregar una especialidad a la lista si no existe otra con el mismo nombre
        // Devuelve true si se agrego, y false si ya estaba registrada
        public static bool AgregarEspecialidad(Especialidades especialidad)
        {
            // Verificacion para no tener nombres duplicados
            if (listaDeEspecialidades.Any(e => e.Nombre.Equals(especialidad.Nombre, System.StringComparison.OrdinalIgnoreCase)))
            {
                return false; // Ya existe, no se agrega.
            }

            listaDeEspecialidades.Add(especialidad);
            return true; // Se agregó correctamente.
        }

        /// <summary>
        /// Devuelve una copia de la lista de todas las especialidades.
        /// </summary>
        /// <returns>Una lista de especialidades.</returns>
        // Devuelve una nueva lista con todas las especialidades guardadas
        // Se devuelve una copia para evitar que modifiquen la lista original desde fuera
        public static List<Especialidades> ObtenerEspecialidades()
        {
            // Devolvemos una nueva lista para que la lista original no pueda ser modificada desde fuera.
            return new List<Especialidades>(listaDeEspecialidades);
        }
    }
}