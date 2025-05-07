import { Route, Routes } from "react-router-dom";
import HomePage from "../pages/HomePage";
import LoginPage from "../pages/LoginPage";
import ProtectedRoute from "./ProtectedRoute";
import VisitasPage from "../pages/VisitasPage";
import UltimosRegistrosPage from "../pages/UltimosRegistrosPage";

export const AppRoutes = () => (
    <Routes>
      <Route path="/login" element={<LoginPage />} />

      <Route
        path="/"
        element={
          <ProtectedRoute>
            <HomePage />
          </ProtectedRoute>
        }
      />

      <Route
        path="/visitas"
        element={
          <ProtectedRoute>
            <VisitasPage />
          </ProtectedRoute>
        }
      />

      <Route
        path="/ultimosRegistros"
        element={
          <ProtectedRoute>
            <UltimosRegistrosPage />
          </ProtectedRoute>
        }
      />


    </Routes>
);