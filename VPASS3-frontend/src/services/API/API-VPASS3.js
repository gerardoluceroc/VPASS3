export const URL_SERVER = "http://localhost:5113"

export const url_loginSession = `${URL_SERVER}/auth/login`
export const path_getAllSentidos = `${URL_SERVER}/direction/all`
export const path_getAllLugaresEstacionamiento = `${URL_SERVER}/ParkingSpot/all`
export const path_getAllTiposVisita = `${URL_SERVER}/VisitType/all`

/* Bitacora Incidencias */
export const path_getAllBitacoraIncidencias = `${URL_SERVER}/AuditLog/all`

/* Estacionamiento */
export const path_getAllEstacionamientos = `${URL_SERVER}/ParkingSpot/all`
export const path_updateEstacionamiento = `${URL_SERVER}/ParkingSpot/Update`
export const path_createEstacionamiento = `${URL_SERVER}/ParkingSpot/create`

/* Lista Negra */
export const path_getAllListaNegra = `${URL_SERVER}/Blacklist/all`
export const path_updateListaNegra = `${URL_SERVER}/Blacklist/Update`
export const path_createListaNegra = `${URL_SERVER}/Blacklist/create`
export const path_deleteListaNegraPorIdPersona = `${URL_SERVER}/Blacklist/deleteByVisitorId`

/* SubZona */
export const path_getAllSubZonas = `${URL_SERVER}/ZoneSection/all`
export const path_createSubZona = `${URL_SERVER}/ZoneSection/create`
export const path_updateSubZona = `${URL_SERVER}/ZoneSection/update`
export const path_deleteSubZona = `${URL_SERVER}/ZoneSection/delete`

/* Visita */
export const path_getAllVisitas = `${URL_SERVER}/Visit/all`
export const path_getVisitaById = `${URL_SERVER}/Visit/` // + id
export const path_createVisita = `${URL_SERVER}/Visit/create`
export const path_getReportePorRangoDeFechas = `${URL_SERVER}/Visit/export/excel/byDates`
export const path_getReportePorRut = `${URL_SERVER}/Visit/export/excel/byRut`

/* Visitante */
export const path_getAllVisitantes = `${URL_SERVER}/Visitor/all`
export const path_getVisitanteByIdentificationNumber = `${URL_SERVER}/Visitor/idnumber/` // + rut ó pasaporte
export const path_createVisitante = `${URL_SERVER}/Visitor/create`

/* Zona */
export const path_getAllZonas = `${URL_SERVER}/zone/all`
export const path_createZona = `${URL_SERVER}/Zone/create`
export const path_updateZona = `${URL_SERVER}/Zone/update`
export const path_deleteZona = `${URL_SERVER}/Zone/delete`
export const path_getZonaById = `${URL_SERVER}/Zone` // + / + id