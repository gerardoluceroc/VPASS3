import { Route, Routes } from "react-router-dom";
import HomePage from "../pages/HomePage";
import LoginPage from "../pages/LoginPage";
import ProtectedRoute from "./ProtectedRoute";
import VisitasPage from "../pages/VisitasPage";
import UltimosRegistrosPage from "../pages/UltimosRegistrosPage";
import { RUTA_BITACORA_INCIDENCIAS, RUTA_BITACORA_USO_ESTACIONAMIENTO, RUTA_DESCARGAR_REGISTROS, RUTA_GESTION_ENCOMIENDAS, RUTA_GESTION_ESPACIOS_COMUNES, RUTA_GESTION_ESTACIONAMIENTO, RUTA_GESTION_ZONAS, RUTA_HOME, RUTA_LISTA_NEGRA, RUTA_LOGIN, RUTA_NUEVA_VISITA, RUTA_ULTIMOS_REGISTROS } from "../utils/rutasCliente";
import GestionEstacionamientoPage from "../pages/GestionEstacionamientoPage";
import BitacoraIncidenciasPage from "../pages/BitacoraIncidenciasPage";
import ListaNegraPage from "../pages/ListaNegraPage";
import DescargarRegistrosPage from "../pages/DescargarRegistrosPage";
import GestionZonasPage from "../pages/GestionZonasPage";
import BitacoraUsoEstacionamientoPage from "../pages/BitacoraUsoEstacionamientoPage";
import GestionEspaciosComunesPage from "../pages/GestionEspaciosComunesPage";
import GestionEncomiendasPage from "../pages/GestionEncomiendasPage";

export const AppRoutes = () => (
  <Routes>
    <Route path={RUTA_LOGIN} element={<LoginPage />} />

    <Route
      path={RUTA_HOME}
      element={
        <ProtectedRoute>
          <HomePage />
        </ProtectedRoute>
      }
    />

    <Route
      path={RUTA_NUEVA_VISITA}
      element={
        <ProtectedRoute>
          <VisitasPage />
        </ProtectedRoute>
      }
    />

    <Route
      path={RUTA_ULTIMOS_REGISTROS}
      element={
        <ProtectedRoute>
          <UltimosRegistrosPage />
        </ProtectedRoute>
      }
    />

    <Route
      path={RUTA_GESTION_ESTACIONAMIENTO}
      element={
        <ProtectedRoute>
          <GestionEstacionamientoPage />
        </ProtectedRoute>
      }
    />

    <Route
      path={RUTA_BITACORA_INCIDENCIAS}
      element={
        <ProtectedRoute>
          <BitacoraIncidenciasPage />
        </ProtectedRoute>
      }
    />

    <Route
      path={RUTA_LISTA_NEGRA}
      element={
        <ProtectedRoute>
          <ListaNegraPage/>
        </ProtectedRoute>
      }
    />

    <Route
      path={RUTA_DESCARGAR_REGISTROS}
      element={
        <ProtectedRoute>
          <DescargarRegistrosPage/>
        </ProtectedRoute>
      }
    />

    {/* <Route
      path={RUTA_GESTION_ZONAS}
      element={
        <ProtectedRoute>
          <GestionZonasPage/>
        </ProtectedRoute>
      }
    /> */}

    <Route
      path={RUTA_BITACORA_USO_ESTACIONAMIENTO}
      element={
        <ProtectedRoute>
          <BitacoraUsoEstacionamientoPage/>
        </ProtectedRoute>
      }
    />

    <Route
      path={RUTA_GESTION_ESPACIOS_COMUNES}
      element={
        <ProtectedRoute>
          <GestionEspaciosComunesPage/>
        </ProtectedRoute>
      }
    />

    <Route
      path={RUTA_GESTION_ENCOMIENDAS}
      element={
        <ProtectedRoute>
          <GestionEncomiendasPage />
        </ProtectedRoute>
      }
    />
  </Routes>
);