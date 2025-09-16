package org.texttechnologylab.vasililab.rest;

import com.mongodb.BasicDBObject;
import com.mongodb.client.MongoCursor;
import com.mongodb.client.model.Filters;
import io.swagger.annotations.*;
import org.bson.Document;
import org.bson.conversions.Bson;
import org.bson.types.ObjectId;
import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;
import org.texttechnologylab.utilities.documentation.SwaggerParser;
import org.texttechnologylab.utilities.helper.SparkUtils;
import org.texttechnologylab.vasililab.database.MongoDBConnectionHandler;
import spark.Request;
import spark.Response;
import spark.Spark;

import javax.ws.rs.*;
import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Date;
import java.util.List;
import java.util.stream.Collectors;

import static spark.Spark.get;


@SwaggerDefinition(host = "api.vasililab.texttechnologylab.org",//"localhost:4567", //
        info = @Info(description = "TTLab API for Va.Si.Li-Lab", //
                version = "v1.0", //
                title = "TTLab Parliament API", //
                contact = @Contact(name = "Giuseppe Abrami", url = "https://www.texttechnologylab.org/team/giuseppe-abrami/")), //
        schemes = {SwaggerDefinition.Scheme.HTTP} //
)

@Api(value = "Va.Si.Li-Lab")
@Path("/")
public class Methods {

    private MongoDBConnectionHandler dbHandler = null;

    public Methods(MongoDBConnectionHandler dbhandler) {
        this.dbHandler = dbhandler;
    }

    public MongoDBConnectionHandler getDBHandler() {
        return this.dbHandler;
    }

    public void update() {
        MongoCursor<Document> rDocuments = dbHandler.doQueryIterator("{}", "scenarios");

        rDocuments.forEachRemaining(d -> {
            d.put("enable", false);
            dbHandler.update(d, "scenarios");
        });

    }

    public void init() {

//        update();

        try {
            // Build swagger json description
            final String swaggerJson = SwaggerParser.getSwaggerJson("org.texttechnologylab.vasililab");
            get("/swagger", (req, res) -> {
                return swaggerJson;
            });

        } catch (Exception e) {
            System.err.println(e);
            e.printStackTrace();
        }

        Spark.options("/*", (request, response) -> {
            String accessControlRequestHeaders = request
                    .headers("Access-Control-Request-Headers");
            if (accessControlRequestHeaders != null) {
                response.header("Access-Control-Allow-Headers",
                        accessControlRequestHeaders);
            }

            String accessControlRequestMethod = request
                    .headers("Access-Control-Request-Method");
            if (accessControlRequestMethod != null) {
                response.header("Access-Control-Allow-Methods",
                        accessControlRequestMethod);
            }
            return "OK";
        });

        Spark.before("*", (req, res) -> {

            SimpleDateFormat sdf = new SimpleDateFormat("dd.MM.YYYY HH:mm:ss");

            System.out.println(sdf.format(new Date((System.currentTimeMillis()))) + "\t" + req.ip() + "\t" + req.pathInfo() + "\t" + req.queryString());

        });


        get("/", (req, res) -> {
            res.redirect("/doku/");
            return null;
        });

        Spark.post("/scene", "application/json", (req, res) -> createScenario(req, res));
        Spark.put("/scene", "application/json", (req, res) -> updateScenario(req, res));
        Spark.get("/scenes", "application/json", (req, res) -> listScenarios(req, res));
        Spark.get("/scenesv2", "application/json", (req, res) -> listScenariosv2(req, res));

        Spark.post("/role", "application/json", (req, res) -> createRole(req, res));
        Spark.put("/role", "application/json", (req, res) -> updateRole(req, res));
        Spark.get("/roles", "application/json", (req, res) -> listRoles(req, res));

        Spark.get("/rolesv2", "application/json", (req, res) -> listRolesv2(req, res));

        Spark.get("/infos", "application/json", (req, res) -> infos(req, res));
    }

    @POST
    @Path("/scene")
    @Consumes({"application/json"})
    @ApiOperation(value = "Insert an scenario")
    @ApiResponses(value = {
            @ApiResponse(code = 200, message = "Success", response = JSONObject.class),
            @ApiResponse(code = 400, message = "Failure", response = JSONObject.class)
    })
    @ApiImplicitParams(value = {
            @ApiImplicitParam(dataType = "string", name = "name", required = true, paramType = "query", value = "Name of Scenario"),
            @ApiImplicitParam(dataType = "string", name = "shortName", required = true, paramType = "query", value = "Short name of Scenario"),
            @ApiImplicitParam(dataType = "string", name = "enabled", required = true, paramType = "query", value = "enable / disable of Scenario"),
            @ApiImplicitParam(dataType = "string", name = "author", required = true, paramType = "query", value = "Author of scenario"),
            @ApiImplicitParam(dataType = "string", name = "internalName", required = false, paramType = "query", value = "Internal name of scenario"),

    })
    public boolean createScenario(@ApiParam(hidden = true) Request req, @ApiParam(hidden = true) Response res) {

        try {
            String sName = req.queryParams("name");
            String sShortName = req.queryParams("shortName");
            String sAuthor = req.queryParams("author");
            boolean bEnable = Boolean.valueOf(req.queryParams().contains("enable") ? req.queryParams("enable") : "false");
            String sInternalName = req.queryParams("internalName");

            Document d1 = new Document();
            d1.put("name", sName);
            d1.put("shortName", sShortName);
            d1.put("enable", bEnable);
            d1.put("author", sAuthor);
            d1.put("internalName", sInternalName);

            this.getDBHandler().insert(d1, "scenarios");

            return true;
        } catch (Exception e) {
            e.printStackTrace();
            return false;
        }
    }

    @PUT
    @Path("/scene")
    @Consumes({"application/json"})
    @ApiOperation(value = "Update an scenario")
    @ApiResponses(value = {
            @ApiResponse(code = 200, message = "Success", response = JSONObject.class),
            @ApiResponse(code = 400, message = "Failure", response = JSONObject.class)
    })
    @ApiImplicitParams(value = {
            @ApiImplicitParam(dataType = "string", name = "id", required = true, paramType = "query", value = "ID of Scenario"),
            @ApiImplicitParam(dataType = "string", name = "name", required = false, paramType = "query", value = "Name of Scenario"),
            @ApiImplicitParam(dataType = "string", name = "shortName", required = false, paramType = "query", value = "Short name of Scenario"),
            @ApiImplicitParam(dataType = "string", name = "author", required = false, paramType = "query", value = "Author of scenario"),
            @ApiImplicitParam(dataType = "string", name = "enabled", required = false, paramType = "query", value = "enable / disable scenario"),
            @ApiImplicitParam(dataType = "string", name = "internalName", required = false, paramType = "query", value = "Internal name of scenario"),

    })
    public boolean updateScenario(@ApiParam(hidden = true) Request req, @ApiParam(hidden = true) Response res) {

        try {
            String sID = req.queryParams("id");
            String sName = req.queryParams().contains("name") ? req.queryParams("name") : "";
            String sShortName = req.queryParams().contains("shortName") ? req.queryParams("shortName") : "";
            String sAuthor = req.queryParams().contains("author") ? req.queryParams("author") : "";
            String sInternalName = req.queryParams().contains("internalName") ? req.queryParams("internalName") : "";

            if (sID.length() == 0) {
                throw new Exception("Param 'id' is required!");
            }

            Document pDocument = this.getDBHandler().getObject(sID, "scenarios");

            if (sName.length() > 0) {
                pDocument.put("name", sName);
            }
            if (sShortName.length() > 0) {
                pDocument.put("shortName", sShortName);
            }
            if (sAuthor.length() > 0) {
                pDocument.put("author", sAuthor);
            }
            if (req.queryParams().contains("enabled")) {
                pDocument.put("enable", Boolean.valueOf(req.queryParams("enabled")));
            }
            if (sInternalName.length() > 0) {
                pDocument.put("internalName", sInternalName);
            }

            this.getDBHandler().update(pDocument, "scenarios");

            return SparkUtils.prepareReturnSuccess(res, new JSONObject().put("result", pDocument.toJson()));
        } catch (Exception e) {
            e.printStackTrace();
            return false;
        }
    }

    @POST
    @Path("/role")
    @Consumes({"application/json"})
    @ApiOperation(value = "Insert an scenario")
    @ApiResponses(value = {
            @ApiResponse(code = 200, message = "Success", response = JSONObject.class),
            @ApiResponse(code = 400, message = "Failure", response = JSONObject.class)
    })
    @ApiImplicitParams(value = {
            @ApiImplicitParam(dataType = "string", name = "id", required = true, paramType = "query", value = "scenario id"),
            @ApiImplicitParam(dataType = "string", name = "name", required = true, paramType = "query", value = "name of the role"),
            @ApiImplicitParam(dataType = "string", name = "description", required = true, paramType = "query", value = "description of the role"),
            @ApiImplicitParam(dataType = "string", name = "mode", required = true, paramType = "query", value = "mode of the role"),
            @ApiImplicitParam(dataType = "string", name = "spawnPosition", required = false, paramType = "query", value = "spawnPosition of the role"),

    })
    public boolean createRole(@ApiParam(hidden = true) Request req, @ApiParam(hidden = true) Response res) {

        try {
            String sName = req.queryParams("name");
            String sMode = req.queryParams("mode");
            String sDescription = req.queryParams("description");
            String sSpawnPosition = req.queryParams("spawnPosition");
            String sID = req.queryParams("id");

            Document role = new Document();
            role.put("name", sName);
            role.put("mode", sMode);
            role.put("description", sDescription);
            role.put("spawnPosition", sSpawnPosition);

            Document scenario = this.getDBHandler().getObject(sID, "scenarios");

            if (scenario != null) {

                List<Document> roles = new ArrayList<>(0);

                if (scenario.containsKey("roles")) {
                    roles = scenario.getList("roles", Document.class);
                }

                roles.add(role);
                scenario.put("roles", roles);
                this.getDBHandler().update(scenario, "scenarios");
            }


            return true;
        } catch (Exception e) {
            e.printStackTrace();
            return false;
        }
    }

    @PUT
    @Path("/role")
    @Consumes({"application/json"})
    @ApiOperation(value = "Update a role")
    @ApiResponses(value = {
            @ApiResponse(code = 200, message = "Success", response = JSONObject.class),
            @ApiResponse(code = 400, message = "Failure", response = JSONObject.class)
    })
    @ApiImplicitParams(value = {
            @ApiImplicitParam(dataType = "string", name = "id", required = true, paramType = "query", value = "scenario id"),
            @ApiImplicitParam(dataType = "string", name = "identifier", required = true, paramType = "query", value = "identifier of the role"),
            @ApiImplicitParam(dataType = "string", name = "name", required = false, paramType = "query", value = "name of the role"),
            @ApiImplicitParam(dataType = "string", name = "description", required = false, paramType = "query", value = "description of the role"),
            @ApiImplicitParam(dataType = "string", name = "mode", required = false, paramType = "query", value = "mode of the role"),
            @ApiImplicitParam(dataType = "string", name = "spawnPosition", required = false, paramType = "query", value = "spawnPosition of the role"),

    })
    public boolean updateRole(@ApiParam(hidden = true) Request req, @ApiParam(hidden = true) Response res) throws JSONException {

        try {


            String sIdentifier = req.queryParams("identifier");
            String sName = req.queryParams().contains("name") ? req.queryParams("name") : "";
            String sMode = req.queryParams().contains("mode") ? req.queryParams("mode") : "";
            String sDescription = req.queryParams().contains("description") ? req.queryParams("description") : "";
            String sSpawnPosition = req.queryParams().contains("spawnPosition") ? req.queryParams("spawnPosition") : "";
            String sID = req.queryParams("id");

            Document scenario = this.getDBHandler().getObject(sID, "scenarios");

            List<Document> roles = new ArrayList<>(0);

            Document role = new Document();

            if (scenario.containsKey("roles")) {
                roles = scenario.getList("roles", Document.class);

                role = roles.stream().filter(r -> {
                    return r.getString("name").equalsIgnoreCase(sIdentifier);
                }).collect(Collectors.toList()).get(0);
            }


            if (sName.length() > 0) {
                role.put("name", sName);
            }
            if (sMode.length() > 0) {
                role.put("mode", sMode);
            }
            if (sDescription.length() > 0) {
                role.put("description", sDescription);
            }
            if (sSpawnPosition.length() > 0) {
                role.put("spawnPosition", sSpawnPosition);
            }

            roles.remove(role);
            roles.add(role);

            scenario.put("roles", roles);
            this.getDBHandler().update(scenario, "scenarios");

            return SparkUtils.prepareReturnSuccess(res, new JSONObject().put("result", scenario.toJson()));
        } catch (Exception e) {
            e.printStackTrace();
            return SparkUtils.prepareReturnFailure(res, e.getMessage(), e.getStackTrace());
        }
    }

    @GET
    @Path("/scenes")
    @Consumes({"application/json"})
    @ApiOperation(value = "Returns all scenarios")
    @ApiResponses(value = {
            @ApiResponse(code = 200, message = "Success", response = JSONObject.class),
            @ApiResponse(code = 400, message = "Failure", response = JSONObject.class)
    })
    @ApiImplicitParams(value = {
            @ApiImplicitParam(dataType = "boolean", name = "disabled", required = false, paramType = "query", value = "show also disabled scenarios"),
            @ApiImplicitParam(dataType = "boolean", name = "small", required = false, paramType = "query", value = "small results")
    })
    public boolean listScenarios(@ApiParam(hidden = true) Request req, @ApiParam(hidden = true) Response res) throws JSONException {

        try {

            boolean bDisable = Boolean.valueOf(req.queryParams().contains("disabled") ? req.queryParams("disabled") : "false");
            boolean bSmall = Boolean.valueOf(req.queryParams().contains("small") ? req.queryParams("small") : "false");

            MongoCursor<Document> rDocuments = null;

            if (bDisable) {
                rDocuments = this.dbHandler.doQueryIterator("{}", "scenarios");
            } else {
                rDocuments = this.dbHandler.doQueryIterator("{enable : true}", "scenarios");
            }

            JSONArray rArray = new JSONArray();

            rDocuments.forEachRemaining(d -> {
                try {
                    d.put("id", String.valueOf(d.getObjectId("_id")));
                    d.remove("_id");
                    JSONObject rObject = new JSONObject(d.toJson());
                    if (bSmall) {
                        rObject.remove("roles");
                    }

                    rArray.put(rObject);
                } catch (JSONException e) {
                    throw new RuntimeException(e);
                }
            });

            return SparkUtils.prepareReturnSuccess(res, new JSONObject().put("result", rArray));

        } catch (Exception e) {
            e.printStackTrace();
            return SparkUtils.prepareReturnFailure(res, e.getMessage(), e.getStackTrace());
        }

    }

    @GET
    @Path("/scenesv2")
    @Consumes({"application/json"})
    @ApiOperation(value = "Returns all scenarios")
    @ApiResponses(value = {
            @ApiResponse(code = 200, message = "Success", response = JSONObject.class),
            @ApiResponse(code = 400, message = "Failure", response = JSONObject.class)
    })
    @ApiImplicitParams(value = {
            @ApiImplicitParam(dataType = "boolean", name = "disabled", required = false, paramType = "query", value = "show also disabled scenarios"),
            @ApiImplicitParam(dataType = "boolean", name = "small", required = false, paramType = "query", value = "small results")
    })
    public boolean listScenariosv2(@ApiParam(hidden = true) Request req, @ApiParam(hidden = true) Response res) throws JSONException {

        try {

            boolean bDisable = Boolean.valueOf(req.queryParams().contains("disabled") ? req.queryParams("disabled") : "false");
            boolean bSmall = Boolean.valueOf(req.queryParams().contains("small") ? req.queryParams("small") : "false");

            MongoCursor<Document> rDocuments = null;

            if (bDisable) {
                rDocuments = this.dbHandler.doQueryIterator("{}", "scenarios-languages");
            } else {
                rDocuments = this.dbHandler.doQueryIterator("{enable : true}", "scenarios-languages");
            }

            JSONArray rArray = new JSONArray();

            rDocuments.forEachRemaining(d -> {
                try {
                    d.put("id", String.valueOf(d.getObjectId("_id")));
                    d.remove("_id");
                    JSONObject rObject = new JSONObject(d.toJson());
                    if (bSmall) {
                        rObject.remove("roles");
                    }

                    rArray.put(rObject);
                } catch (JSONException e) {
                    throw new RuntimeException(e);
                }
            });

            return SparkUtils.prepareReturnSuccess(res, new JSONObject().put("result", rArray));

        } catch (Exception e) {
            e.printStackTrace();
            return SparkUtils.prepareReturnFailure(res, e.getMessage(), e.getStackTrace());
        }

    }

    @GET
    @Path("/roles")
    @Consumes({"application/json"})
    @ApiOperation(value = "Returns all roles")
    @ApiResponses(value = {
            @ApiResponse(code = 200, message = "Success", response = JSONObject.class),
            @ApiResponse(code = 400, message = "Failure", response = JSONObject.class)
    })
    @ApiImplicitParams(value = {
            @ApiImplicitParam(dataType = "string", name = "scene", required = false, paramType = "query", value = "scenario id")
    })
    public boolean listRoles(@ApiParam(hidden = true) Request req, @ApiParam(hidden = true) Response res) throws JSONException {

        try {

            String sScene = req.queryParams("scene");

            List<Document> rDocuments = new ArrayList<>(0);

            if (sScene.length() > 0) {
                rDocuments.add(this.dbHandler.getObject(sScene, "scenarios"));
            } else {
                this.dbHandler.doQueryIterator("{}", "scenarios").forEachRemaining(d -> {
                    rDocuments.add(d);
                });
            }


            JSONArray rArray = new JSONArray();

            rDocuments.stream().forEach(d -> {
                try {
                    for (Document roles : d.getList("roles", Document.class)) {
                        JSONObject rObject = new JSONObject(roles.toJson());
                        rArray.put(rObject);
                    }

                } catch (JSONException e) {
                    throw new RuntimeException(e);
                }
            });

            return SparkUtils.prepareReturnSuccess(res, new JSONObject().put("result", rArray));

        } catch (Exception e) {
            e.printStackTrace();
            return SparkUtils.prepareReturnFailure(res, e.getMessage(), e.getStackTrace());
        }

    }

    @GET
    @Path("/rolesv2")
    @Consumes({"application/json"})
    @ApiOperation(value = "Returns all roles")
    @ApiResponses(value = {
            @ApiResponse(code = 200, message = "Success", response = JSONObject.class),
            @ApiResponse(code = 400, message = "Failure", response = JSONObject.class)
    })
    @ApiImplicitParams(value = {
            @ApiImplicitParam(dataType = "string", name = "scene", required = true, paramType = "query", value = "scenario id")
    })
    public boolean listRolesv2(@ApiParam(hidden = true) Request req, @ApiParam(hidden = true) Response res) throws JSONException {

        try {

            String sScene = req.queryParams("scene");

            Document rDocument = new Document();

            if (sScene != null && sScene.length() > 0) {
                rDocument = this.dbHandler.getObject(sScene, "scenarios-languages").get("roles", Document.class);
            } else {
                SparkUtils.prepareReturnFailure(res, new JSONObject().put("message", "The param scene is required"));
            }
            return SparkUtils.prepareReturnSuccess(res, new JSONObject().put("result", rDocument));

        } catch (Exception e) {
            e.printStackTrace();
            return SparkUtils.prepareReturnFailure(res, e.getMessage(), e.getStackTrace());
        }

    }

    @GET
    @Path("/infos")
    @Consumes({"application/json"})
    @ApiOperation(value = "Returns all infos")
    @ApiResponses(value = {
            @ApiResponse(code = 200, message = "Success", response = JSONObject.class),
            @ApiResponse(code = 400, message = "Failure", response = JSONObject.class)
    })
    public boolean infos(@ApiParam(hidden = true) Request req, @ApiParam(hidden = true) Response res) throws JSONException {

        try {
            List<Document> rDocuments = new ArrayList<>(0);

            this.dbHandler.doQueryIterator("{}", "globalInfos").forEachRemaining(d -> {
                rDocuments.add(d);
            });

            JSONArray rArray = new JSONArray();

            for (Document doc: rDocuments)
            {
                rArray.put(new JSONObject(doc));
            }

            return SparkUtils.prepareReturnSuccess(res, new JSONObject().put("result", rArray));

        } catch (Exception e) {
            e.printStackTrace();
            return SparkUtils.prepareReturnFailure(res, e.getMessage(), e.getStackTrace());
        }

    }
}
