import { useSelector } from "react-redux";
import { Navigate } from "react-router-dom";

const ProtectedRoute = ({ children }) => {
  const { authenticated } = useSelector((state) => state.user);

  // Si no está autenticado, redirige a /login
  if (!authenticated) {
    return <Navigate to="/login" replace />;
  }

  // Si está autenticado, muestra el contenido protegido
  return children;
};

export default ProtectedRoute;
