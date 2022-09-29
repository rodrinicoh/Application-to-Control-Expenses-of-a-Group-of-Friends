using Exceptions;
using GestorAlmacenamiento;
using GestorUsuarios.Modelo;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grafo;
using Colecciones;
using GestorDeudas.Presentador;

namespace GestorDeudas.Modelo
{
    public class DeudasManager
    {
        private DeudasPresentador deudasPresentador;
        private UsuarioManager usuariosManager;
        private Coleccion<Usuario> listaUsuarios;

        private GestorAlmacenamiento.Almacenamiento database;

        private List<Deuda> listaAcreedoresUltimoDeudor;

        const char userSeparator = '-';


        private Coleccion<Deuda> deudas;

        const string textErrorAdministrador = "\n Por favor, comuniquese con el administrador\n";

        public DeudasManager(DeudasPresentador deudasPresentador)
        {
            this.deudasPresentador = deudasPresentador;
            deudas = new ColeccionLista<Deuda>();

        }
        private void mostrarMensaje(string mensaje, bool shareButtonEnable)
        {
            deudasPresentador.mostrarMensaje(mensaje, shareButtonEnable);
        }

        private void mostrarMensajeError(string mensaje)
        {
            mostrarMensaje(mensaje + textErrorAdministrador, false);
        }

        private void mostrarMensajeCorrecto(string mensaje)
        {
            mostrarMensaje(mensaje, true);
        }

        public void setearUsuarioManagerYAlmacenamiento(UsuarioManager usuarioManager, Almacenamiento database)
        {
            this.usuariosManager = usuarioManager;
            this.database = database;
            try
            {
                database.cargarTodasLasDeudas(deudas, usuariosManager.obtenerUsuariosVerDeudas());
            }
            catch (DatabaseException e)
            {
                lanzarDatabaseExcepcion("setearUsuarioManager()");
            }

        }

        private void lanzarDatabaseExcepcion(string metodo)
        {
            string texto = "Hay un error en la base de datos en metodo: " + metodo;
            mostrarMensajeError(texto);
        }


        /**
         * Utilizo la variable incluir para incluir o no en la lista al usuario tech fashion.
        */
        private List<Usuario> obtenerUsuarios(bool incluirTech)
        {
            List<Usuario> usuarios = new List<Usuario>();
            foreach (Deuda deuda in deudas.obtenerIterable())
            {
                Usuario deudor = deuda.obtenerDeudor();
                int dni = deudor.obtenerDni();
                //Debug.Log("Deuda deudor:" + dni + " acreedor:" + deuda.obtenerAcreedor().obtenerDni());

                //Si aun no se agrego el usuario a la lista de usuarios deudores, lo agrego.
                if (!usuarios.Contains(deudor))
                {
                    if (usuariosManager.esUsuarioTech(dni)) //Si el usuario deudor actual es tech, me fijo con la vriable incluirtech si hay que incluirlo
                    {
                        if (incluirTech)
                            usuarios.Add(deudor);
                    }
                    else
                        usuarios.Add(deudor);
                }

            }
            return usuarios;
        }

        public void obtenerUsuariosParticipantes()
        {
            List<Usuario> usuarios = obtenerUsuarios(false);
            deudasPresentador.actualizarUsuarios(usuarios);
        }

        public void obtenerUsuariosDeudores()
        {
            List<Usuario> usuarios = obtenerUsuarios(true);
            deudasPresentador.actualizarUsuarios(usuarios);
        }

        public void obtenerDeudasUsuario(int dniDeudor)
        {
            Usuario deudor = usuariosManager.obtenerUsuario(dniDeudor);
            deudasPresentador.actualizarDeudasDeudor(deudor.obtenerDeudas());
        }

        private Usuario obtenerUsuario(int dni)
        {
            return usuariosManager.obtenerUsuario(dni);
        }

        private void eliminarDeuda(Usuario usuario, Deuda deuda)
        {
            usuario.liquidarDeuda(deuda);
            deudas.eliminar(deuda);
            database.eliminarDeuda(deuda.obtenerIDDeuda(), usuario.obtenerDni());
        }

        private void actualizarDeuda(Deuda deuda, float monto)
        {
            deuda.setMonto(monto);
            database.actualizarDeuda(deuda);
        }

        private string armarMensajeDeuda(Usuario deudor, Usuario acreedor, float monto, float montoViejo)
        {
            string msg;
            msg = "Se liquidaron $" + monto + " de la siguiente deuda:\n";
            msg += "Deudor: " + deudor.obtenerDniNombreApellido(userSeparator) + "\n";
            msg += "Acreedor: " + acreedor.obtenerDniNombreApellido(userSeparator) + "\n";
            msg += "Monto: $" + montoViejo  + "\n";
            return msg;
        }

        private string liquidarDeuda(Deuda deuda, Usuario usuarioDeudor, float monto)
        {
            String msg = "";
            float montoViejo = deuda.getMonto();
            Usuario usuarioAcreedor = deuda.obtenerAcreedor();

            if (monto < montoViejo) //Solo se saldan deudas totales.
                actualizarDeuda(deuda, montoViejo - monto);

            if (monto >= montoViejo)
                eliminarDeuda(usuarioDeudor, deuda);

            msg = armarMensajeDeuda(usuarioDeudor, usuarioAcreedor, monto, montoViejo);
            if (monto > montoViejo) //Se liquida mas de lo adeudado, sobra.
                msg += "Le sobran $" + (monto - montoViejo) + "\n";
            return msg;
        }

        public void liquidarDeuda(int deudor, int idDeuda, float monto, bool total)
        {
            String msg = "";

            bool seLiquido = false;
            Usuario usuarioDeudor = obtenerUsuario(deudor);
            foreach (Deuda deuda in usuarioDeudor.obtenerDeudas().obtenerIterable())
            {
                if (deuda.obtenerIDDeuda() == idDeuda)
                {
                    if (total) //Si se eligio liquidar la deuda total, se liquida total.
                        msg = liquidarDeuda(deuda, usuarioDeudor,deuda.getMonto());
                    else //Sino se pasa el monto que se recibe.
                        msg = liquidarDeuda(deuda, usuarioDeudor,monto);
                    seLiquido = true;
                    deudasPresentador.mostrarHistorial(msg);
                    break;
                }
            }

            if (!seLiquido) //No se encontro la deuda, error.
            {
                msg = "Hubo un error al borrar la deuda Nº: " + idDeuda;
                msg += " \n Acreedor: " + usuarioDeudor.obtenerDniNombreApellido(userSeparator) + "\n";
                mostrarMensajeError(msg);
            }
            else
                mostrarMensajeCorrecto(msg);
        }

        public bool esDeudaUrgente(Deuda deuda)
        {
            return deuda.GetType() == typeof(DeudaUrgente);
        }

        public void crearDeuda(Usuario deudor, Usuario acreedor, float montoPersona, bool urgente)
        {
            Deuda deuda;
            if (urgente)
                deuda = new DeudaUrgente(deudor, acreedor, montoPersona, 0);
            else
                deuda = new Deuda(deudor, acreedor, montoPersona, 0);
            database.guardarDeuda(deuda);
            deudor.agregarDeuda(deuda);
            agregarDeuda(deuda);

        }

        public Coleccion<Deuda> obtenerDeudas()
        {
            
            return deudas;
        }
    
        private void agregarDeuda(Deuda deuda){
            deudas.agregar(deuda);
        }

        public bool simplificarDeuda()
        {
            Graph<Usuario, Deuda> g = new GrafoNoDirigidoConListaDeArcos<Usuario, Deuda>();
            crearGrafo(g);
            string msg = ""; 
            if (simplificar(g))
            {
                msg = "Se simplificaron las deudas correspondientes";
                deudasPresentador.mostrarHistorial(msg);
            }
            else
            {
                msg = "No hay deudas para simplificar";
            }
            mostrarMensajeCorrecto(msg);
            return true;
        }

        private void crearGrafo(Graph<Usuario, Deuda> g)
        {
            List<Vertex<Usuario>> arr_usuarios = new List<Vertex<Usuario>>();
            Coleccion<Usuario> usuarios = new ColeccionLista<Usuario>();
            usuarios = usuariosManager.obtenerUsuarios();
            for (int j = 0; j < usuarios.longitud(); j++)
            {
                arr_usuarios.Add(g.insertVertex(usuarios.get(j)));

            }

            foreach (Vertex<Usuario> w in arr_usuarios)
            {
                for (int i = 0; i < deudas.longitud(); i++)
                {
                    if (string.Equals(deudas.get(i).obtenerDeudor().obtenerDni(), w.element().obtenerDni()))
                    {
                        foreach (Vertex<Usuario> v in arr_usuarios)
                        {
                            if (string.Equals(v.element().obtenerDni(), deudas.get(i).obtenerAcreedor().obtenerDni()))
                                g.insertEdge(w, v, deudas.get(i));
                        }
                    }
                }
            }

            List<Edge<Deuda>> L = g.edges();
            List<Vertex<Usuario>> L2 = g.vertices();
        }

        private bool simplificar(Graph<Usuario, Deuda> g)
        {
            Resultado<Usuario, Deuda> r;
            List<Vertex<Usuario>> l;
            List<Edge<Deuda>> la;
            bool termine = false;
            float min, aux;
            List<Edge<Deuda>> aEliminar = new List<Edge<Deuda>>();
            bool seBorro = false;
            while (termine == false)
            {
                r = isCyclic(g);
                if (r.getListaArcos().Count > 1 && r.getTermine() == true)
                {
                    l = r.getListaVertices();
                    la = r.getListaArcos();
                    foreach (Vertex<Usuario> v in l)
                    {
                        Debug.Log(v.element().obtenerDni());
                    }
                    min = 0;
                    foreach (Edge<Deuda> e in la)
                    {
                        aux = e.element().getMonto();
                        if (aux < min || min == 0)
                            min = aux;
                    }


                    foreach (Edge<Deuda> e in la)
                    {
                        aux = e.element().getMonto();
                        if (aux == min)
                        {
                            aEliminar.Add(e);
                        }
                        e.element().setMonto(aux - min);
                        if (e.element().getMonto() == 0)
                            aEliminar.Add(e);
                        else
                            database.actualizarDeuda(e.element());
                        
                    }

                    if (aEliminar.Count > 0)
                        seBorro = true;

                    foreach (Edge<Deuda> e in aEliminar)
                    {
                        if (e != null && e.element()!=null)
                        {
                            liquidarDeuda(e.element(), e.element().obtenerDeudor(), e.element().getMonto());
                            g.removeEdge(e);
                        }
                    }


                    r.setTermine(false);
                }
                else
                    termine = true;
            }
            return seBorro;

        }

        // A recursive function that uses visited[]
        // and parent to detect cycle in subgraph
        // reachable from vertex v.
        private bool isCyclicUtil(Graph<Usuario, Deuda> G, Vertex<Usuario> i, IDictionary<Vertex<Usuario>, bool> visitados, Vertex<Usuario> parent, Resultado<Usuario, Deuda> resultado)
        {
            // Mark the current node as visited
            visitados.Remove(i);
            visitados.Add(i, true);

            bool result;
            bool hayCiclo = false;
            // Recur for all the vertices
            // adjacent to this vertex
            if (parent != null)
                Debug.Log("Parent: " + parent.element().obtenerDni());
            foreach (Edge<Deuda> e in G.incidentEdges(i))
            {
                Vertex<Usuario> w = G.opposite(i, e);

                if (i.element().obtenerDni() == e.element().obtenerDeudor().obtenerDni())
                {

                    if ((parent != null) && (parent.element().obtenerDni() == e.element().obtenerAcreedor().obtenerDni()))
                    {
                        resultado.getListaArcos().Add(e);
                        resultado.setTermine(true);
                        return true;
                    }
                    // If an adjacent is not visited,
                    // then recur for that adjacent+
                    visitados.TryGetValue(w, out result);
                    if (result == false)
                    {
                        resultado.getListaArcos().Add(e);


                        hayCiclo = isCyclicUtil(G, w, visitados, i, resultado);

                        if (hayCiclo == true)
                        {
                            resultado.getListaVertices().Add(i);
                            resultado.setTermine(true);
                            return true;
                        }
                        else
                        {
                            resultado.getListaArcos().RemoveAt((resultado.getListaArcos().Count - 1));
                        }

                    }

                    // If an adjacent is visited and
                    // not parent of current vertex,
                    // then there is a cycle.
                    else if (w != parent)
                    {
                        resultado.getListaArcos().Add(e);
                        resultado.setTermine(true);
                        return true;
                    }

                }
            }
            return false;
        }

        // Returns true if the graph contains
        // a cycle, else false.
        private Resultado<Usuario, Deuda> isCyclic(Graph<Usuario, Deuda> G)
        {
            Resultado<Usuario, Deuda> resultado = new Resultado<Usuario, Deuda>();
            // Mark all the vertices as not visited
            // and not part of recursion stack
            IDictionary<Vertex<Usuario>, bool> visitados = new Dictionary<Vertex<Usuario>, bool>();
            foreach (Vertex<Usuario> w in G.vertices())
                visitados.Add(w, false);

            bool result;
            // Call the recursive helper function
            // to detect cycle in different DFS trees
            foreach (Vertex<Usuario> v in G.vertices())
            {
                visitados.TryGetValue(v, out result);
                // Don't recur for u if already visited
                if (result == false)
                {
                    resultado = new Resultado<Usuario, Deuda>();
                    if (isCyclicUtil(G, v, visitados, null, resultado))
                    {
                        resultado.setTermine(true);
                        return resultado;
                    }
                }
            }
            resultado.setTermine(false);
            return resultado; //no hay bucle
        }
    }

}