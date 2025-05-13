export const URL_SERVER = "http://localhost:5113"

export const url_loginSession = `${URL_SERVER}/auth/login`
export const path_getAllSentidos = `${URL_SERVER}/direction/all`
export const path_getAllZonas = `${URL_SERVER}/zone/all`
export const path_getAllLugaresEstacionamiento = `${URL_SERVER}/ParkingSpot/all`
export const path_getAllTiposVisita = `${URL_SERVER}/VisitType/all`

/* Estacionamiento */
export const path_getAllEstacionamientos = `${URL_SERVER}/ParkingSpot/all`
export const path_updateEstacionamiento = `${URL_SERVER}/ParkingSpot/Update`


/* Visita */
export const path_getAllVisitas = `${URL_SERVER}/Visit/all`
export const path_getVisitaById = `${URL_SERVER}/Visit/` // + id
export const path_createVisita = `${URL_SERVER}/Visit/create`



/* Visitante */
export const path_getAllVisitantes = `${URL_SERVER}/Visitor/all`
export const path_getVisitanteByIdentificationNumber = `${URL_SERVER}/Visitor/idnumber/` // + rut ó pasaporte
export const path_createVisitante = `${URL_SERVER}/Visitor/create`

/* Estacionamiento */
