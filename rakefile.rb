require 'rubygems'
require 'albacore'

PROJECT_ROOT = Dir.pwd
OUTPUT_DIR = File.join(PROJECT_ROOT, 'Output')

ILMERGE = File.join(PROJECT_ROOT, 'Tools', 'ILMerge.exe')
MSBUILD = "C:/Windows/Microsoft.NET/Framework/v4.0.30319/MSBuild.exe"

SOLUTION = File.join(PROJECT_ROOT, 'Clarity.Util.ConfigTransformer.sln')

@config = 'Release'

desc "Clean the output directory"
task :clean do
	FileUtils.remove_dir(OUTPUT_DIR, true)
end

desc "Compile the project"
msbuild :compile, [:project_file, :target_dir] => :clean do |msb, args|
	msb.command = MSBUILD
	msb.properties  = { "Configuration" => "#{@config}", "OutputPath" => "#{OUTPUT_DIR}" }
	msb.targets :Build
	msb.verbosity = "normal"
	msb.log_level = :verbose
	msb.solution = SOLUTION
end


desc "ilmerge the project into an output exe"
task :merge => :compile do

	outputExe = File.join(OUTPUT_DIR, 'Clarity.Util.ConfigTransformer.exe')
	primaryAssembly = File.join(OUTPUT_DIR, 'Clarity.Util.ConfigTransformer.exe')
	webPubAssembly = File.join(OUTPUT_DIR, 'Microsoft.Web.XmlTransform.Dll')

	command = "#{ILMERGE} /out:#{outputExe} #{primaryAssembly} #{webPubAssembly} /target:exe /targetplatform:v4,C:\\Windows\\Microsoft.NET\\Framework64\\v4.0.30319 /wildcards"
	puts command
	success = sh command
	success
end	