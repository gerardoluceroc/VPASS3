import dayjs from "dayjs";
import { useSelector } from "react-redux";
import { Navigate } from "react-router-dom";

const ProtectedRoute = ({ children }) => {
  const { authenticated, expirationTokenTimestamp } = useSelector((state) => state.user);

  const fechaHoraExpiracionToken = dayjs.unix(expirationTokenTimestamp);
  const fechaHoraActual = dayjs();
  console.log("fechaHoraExpiracionToken", fechaHoraExpiracionToken.format("YYYY-MM-DD HH:mm:ss"));

  if (authenticated) {
    if (fechaHoraActual.isAfter(fechaHoraExpiracionToken)) {
      alert("Su sesión ha expirado, por favor inicie sesión nuevamente.");
      // // Aquí puedes redirigir al usuario a la página de inicio de sesión o realizar otra acción
      // window.location.href = "/login";
      return <Navigate to="/login" replace />;
    } else {
      console.log("Token válido. Mostrando contenido protegido...");
      return children;
    }
  }
  else{
    // Si no está autenticado, redirige a /login
    return <Navigate to="/login" replace />;
  }
};

export default ProtectedRoute;
