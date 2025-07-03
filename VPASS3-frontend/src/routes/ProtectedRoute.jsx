import dayjs from "dayjs";
import { useSelector } from "react-redux";
import { Navigate, useNavigate } from "react-router-dom";
import AlertDialog from "../components/Dialog/AlertDialog/AlertDialog";
import { useState } from "react";
import useLogin from "../hooks/auth/useLogin";
import { RUTA_LOGIN } from "../utils/rutasCliente";

const ProtectedRoute = ({ children }) => {

  const {logoutSession} = useLogin();

  const [showExpiredSessionDialog, setShowExpiredSessionDialog] = useState(false);

  // Función que se ejecutará al presionar "Aceptar" en el AlertDialog
  const handleSessionExpiredAccept = () => {
    logoutSession({sesionExpirada: true});
  };
  const navigate = useNavigate();

  const { authenticated, expirationTokenTimestamp } = useSelector((state) => state.user);
  const fechaHoraExpiracionToken = dayjs.unix(expirationTokenTimestamp);
  const fechaHoraActual = dayjs();

  if (authenticated) {

    // Si el token ha expirado, muestra el AlertDialog y redirige a /login
    if (fechaHoraActual.isAfter(fechaHoraExpiracionToken)) {

      if (!showExpiredSessionDialog) {
        setShowExpiredSessionDialog(true); // Activa el modal
      }

      return (
        <>
          {/* Renderiza el AlertDialog */}
          <AlertDialog
            open={showExpiredSessionDialog}
            onClose={() => setShowExpiredSessionDialog(false)}
            title="Sesión expirada"
            message="Su sesión ha expirado, por favor inicie sesión nuevamente."
            actionPostCierre={handleSessionExpiredAccept}
          />
          {/* Mientras el modal está abierto, no renderiza nada */}
          {null}
        </>
      );

    } else {
      // Si el token no ha expirado, renderiza los hijos
      return children;
    }
  }
  else{
    // Si no está autenticado, redirige a /login
    return <Navigate to={RUTA_LOGIN} replace />;
  }
};

export default ProtectedRoute;