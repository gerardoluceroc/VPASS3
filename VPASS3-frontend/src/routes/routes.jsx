import { Route, Routes } from "react-router-dom";
import HomePage from "../pages/HomePage";
import LoginPage from "../pages/LoginPage";
import ProtectedRoute from "./ProtectedRoute";
import VisitasPage from "../pages/VisitasPage";
import UltimosRegistrosPage from "../pages/UltimosRegistrosPage";
import { RUTA_BITACORA_INCIDENCIAS, RUTA_GESTION_ESTACIONAMIENTO, RUTA_HOME, RUTA_LOGIN, RUTA_NUEVA_VISITA, RUTA_ULTIMOS_REGISTROS } from "../utils/rutasCliente";
import GestionEstacionamientoPage from "../pages/GestionEstacionamientoPage";
import BitacoraIncidenciasPage from "../pages/BitacoraIncidenciasPage";

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
    </Routes>
);